using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageParser.UnitTest
{
    [TestClass]
    public class LanguageFragmentTest
    {

        [TestMethod]
        public void TestLanguegeFragmentParser_Pass_WithArgument()
        {
            var inputString = "[[Game:Next]].[[DateTime:Date]]";

            var languageFragment = LanguageFragment.Parse(inputString);
            var outputString = languageFragment.ToString();

            Assert.AreEqual(inputString, outputString);
        }

        [TestMethod]
        public void TestLanguegeFragmentParser_Pass_NoArguments()
        {
            var inputString = "[[Game:]].[[DateTime]]";

            var languageFragment = LanguageFragment.Parse(inputString);
            var outputString = languageFragment.ToString();

            Assert.AreEqual(inputString.Replace(":", ""), outputString);
        }

        [TestMethod]
        public void TestLanguegeFragmentParser_Pass_AllKinds()
        {
            var inputStrings = new List<string>
            {
                "[[Game:Next]].[[DateTime:Date]]",
                "[[Game:Next]].[[DateTime]]",
                "[[Game]].[[DateTime:Date]]",
                "[[Game]].[[DateTime]]",
            };

            foreach (var inputString in inputStrings)
            {

                var languageFragment = LanguageFragment.Parse(inputString);
                var outputString = languageFragment.ToString();

                Assert.AreEqual(inputString, outputString);
            }
        }

        [TestMethod]
        public void TestLanguegeFragmentParser_Pass_InvalidInput_CannotParse()
        {
            var inputString = "[[].[DateTime]]";

            var languageFragment = LanguageFragment.Parse(inputString);
            var outputString = languageFragment.ToString();

            Assert.AreNotEqual(inputString, outputString);
            Assert.AreEqual(string.Empty, outputString);
        }
    }
}
