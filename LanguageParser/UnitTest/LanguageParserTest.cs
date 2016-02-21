using DataModel.Language;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageParser.UnitTest
{
    [TestClass]
    public class LanguageParserTest
    {

        [TestMethod]
        public void TestLanguageParserRegex_Pass_GetFragments()
        {
            var inputString = @"Some Text {{[[Game:Next]].[[DateTime:Date]]}}@#$%^&*()_1234567890{{}}[[]]{[[QWE:ASD]].[[ASD:NHY]]}}  {{[[Game:Previous]].[[Player:Confirmed]]}}";

            var languageFragments = LanguageParser.GetLanguageFragmentStrings(inputString);

            Assert.AreEqual(languageFragments.Count(), 2);
            Assert.AreEqual(languageFragments[0], "[[Game:Next]].[[DateTime:Date]]");
            Assert.AreEqual(languageFragments[1], "[[Game:Previous]].[[Player:Confirmed]]");
        }

        [TestMethod]
        public void TestParser_NoLanguageFragments_Pass()
        {
            var context = new LanguageableDataModelContext_Mockup();
            context.ManagerId = 1;
            context.InitializeMockup();

            ILanguageParser languageParser = new LanguageParser(context);
            var input = "some string without any language fragments";
            var templateContext = string.Empty;
            var output = languageParser.ParseForManager(input, templateContext, 1).First();

            Assert.AreEqual(input, output);
        }

        [TestMethod]
        public void TestParser_WithMockEntities_NextGame_Pass()
        {
            var context = new LanguageableDataModelContext_Mockup();
            context.ManagerId = 1;
            context.InitializeMockup();

            var templateFormat = "Hello. Next Game is {0}. See you there!";

            var expectedOutput = string.Empty;
            var nextGame = context.Games.FirstOrDefault(g => g.Date > DateTime.Now);
            if (nextGame != null)
            {
                expectedOutput = string.Format(templateFormat, nextGame.Date.ToLongDateString());
            }

            ILanguageParser languageParser = new LanguageParser(context);
            var input = string.Format(templateFormat, "{{[[Game:Next]].[[Self:Date]]}}");
            var templateContext = string.Empty;
            var actualOutput = languageParser.ParseForManager(input, templateContext, 1);

            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [TestMethod]
        public void TestParser_WithMockEntities_RegularsConfirmedForNextGame_Pass()
        {
            var context = new LanguageableDataModelContext_Mockup();
            context.ManagerId = 1;
            context.InitializeMockup();

            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 2, Player_Id = 1 }); // Next Game -> Regular Player
            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 2, Player_Id = 2 }); // Next Game -> Regular Player
            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 1, Player_Id = 2 }); // Prev Game -> Regular Player
            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 2, Player_Id = 6 }); // Next Game -> SPARE Player

            var templateFormat = "Hello. For next games confirmed regulars: {0}. See you there!";

            var expectedOutput = string.Empty;
            var nextGame = context.Games.FirstOrDefault(g => g.Date > DateTime.Now);
            if (nextGame != null)
            {
                var regularPlayerOne = context.Players.First(g => g.Id == 1);
                var regularPlayerTwo = context.Players.First(g => g.Id == 2);

                expectedOutput = string.Format(templateFormat, $"{regularPlayerOne.FullName}{LanguageFragment.ENTITY_SEPARATOR}{regularPlayerTwo.FullName}");
            }

            ILanguageParser languageParser = new LanguageParser(context);
            var input = string.Format(templateFormat, "{{[[Game:Next]].[[Player:Regular]].[[Self]]}}");
            var templateContext = string.Empty;
            var actualOutput = languageParser.ParseForManager(input, templateContext, 1);

            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [TestMethod]
        public void TestParser_WithMockEntities_SparesForGameAfterNext_Pass()
        {
            var context = new LanguageableDataModelContext_Mockup();
            context.ManagerId = 1;
            context.InitializeMockup();

            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 2, Player_Id = 1 });
            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 4, Player_Id = 7 }); // GOOD
            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 1, Player_Id = 2 });
            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 4, Player_Id = 1 });
            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 4, Player_Id = 6 }); // GOOD
            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 3, Player_Id = 6 });

            var templateFormat = "Hello. For next games confirmed regulars: {0}. See you there!";

            var expectedOutput = string.Empty;
            var nextGame = context.Games.Where(g => g.Date > DateTime.Now).Take(2).LastOrDefault();
            if (nextGame != null)
            {
                var spareOne = context.Players.First(g => g.Id == 7);
                var spareTwo = context.Players.First(g => g.Id == 6);

                expectedOutput = string.Format(templateFormat, $"{spareOne.FullName}{LanguageFragment.ENTITY_SEPARATOR}{spareTwo.FullName}");
            }

            ILanguageParser languageParser = new LanguageParser(context);
            var input = string.Format(templateFormat, "{{[[Game:AfterNext]].[[Player:Spare]].[[Self:Name]]}}");
            var templateContext = string.Empty;
            var actualOutput = languageParser.ParseForManager(input, templateContext, 1);

            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [TestMethod]
        public void TestParser_WithMockEntities_SparesForGameAfterNext_WrongManager_NotEqual()
        {
            Assert.AreNotEqual(2, 2);

            var context = new LanguageableDataModelContext_Mockup();
            context.ManagerId = 1;
            context.InitializeMockup();

            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 2, Player_Id = 1 });
            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 4, Player_Id = 7 }); // GOOD
            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 1, Player_Id = 2 });
            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 4, Player_Id = 1 });
            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 4, Player_Id = 6 }); // GOOD
            context.GameConfirmations.Add(new GameConfirmations_Mockup { Game_Id = 3, Player_Id = 1 });

            var templateFormat = "Hello. For next games confirmed regulars: {0}. See you there!";

            var expectedOutput = string.Empty;
            var nextGame = context.Games.Where(g => g.Date > DateTime.Now).Take(2).LastOrDefault();
            if (nextGame != null)
            {
                var spareOne = context.Players.First(g => g.Id == 7);
                var spareTwo = context.Players.First(g => g.Id == 6);

                expectedOutput = string.Format(templateFormat, $"{spareOne.FullName}{LanguageFragment.ENTITY_SEPARATOR}{spareTwo.FullName}");
            }

            ILanguageParser languageParser = new LanguageParser(context);
            var input = string.Format(templateFormat, "{{[[Game:AfterNext]].[[Player:Spare]].[[Self:Name]]}}");

            var templateContext = string.Empty;
            var actualOutput = languageParser.ParseForManager(input, templateContext, 11);

            Assert.AreNotEqual(expectedOutput, actualOutput);
        }


    }
}
