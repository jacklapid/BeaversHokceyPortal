using DataModel.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageParser.UnitTest
{
    internal class LanguageableDataModelContext_Mockup : ILanguageableDataModelContext
    {
        public List<LanguageablePlayer_Mockup> Players { get; set; }

        public List<LanguageableGame_Mockup> Games { get; set; }

        public List<GameConfirmations_Mockup> GameConfirmations { get; set; }

        public int? ManagerId { get; set; }


        public IEnumerable<ILanguagable> GetGames()
        {
            if (this.ManagerId.HasValue)
            {
                return this.Games.Where(g => g.ManagerId == this.ManagerId.Value).ToList();
            }
            else
            {
                return this.Games.ToList();
            }
        }

        public LanguageableDataModelContext_Mockup()
        {
            Players = new List<LanguageablePlayer_Mockup>();
            Games = new List<LanguageableGame_Mockup>();
            GameConfirmations = new List<GameConfirmations_Mockup>();
        }

        public void InitializeMockup()
        {
            #region Games
            var gameId = 1;
            this.Games.Add(new LanguageableGame_Mockup(this)
            {
                Id = gameId++, //1
                Date = DateTime.Now.AddDays(-7),
                ParentLevelEntityId = null,
                ManagerId = this.ManagerId
            });
            this.Games.Add(new LanguageableGame_Mockup(this)
            {
                Id = gameId++, //2
                Date = DateTime.Now.AddDays(1),
                ParentLevelEntityId = null,
                ManagerId = this.ManagerId
            });
            this.Games.Add(new LanguageableGame_Mockup(this)
            {
                Id = gameId++, //3
                Date = DateTime.Now.AddDays(2),
                ParentLevelEntityId = null,
                ManagerId = this.ManagerId.HasValue ? (this.ManagerId.Value + 10) : (int?)null
            });
            this.Games.Add(new LanguageableGame_Mockup(this)
            {
                Id = gameId++, //4
                Date = DateTime.Now.AddDays(10),
                ParentLevelEntityId = null,
                ManagerId = this.ManagerId
            });
            this.Games.Add(new LanguageableGame_Mockup(this)
            {
                Id = gameId++, //5
                Date = DateTime.Now.AddDays(14),
                ParentLevelEntityId = null,
                ManagerId = this.ManagerId
            });

            #endregion Games

            #region Players
            var playerId = 0;
            this.Players.Add(new LanguageablePlayer_Mockup(this)
            {
                Id = ++playerId, //1
                Name = "Regular",
                Lastname = "Player " + playerId,
                IsRegular = true,
                IsGoalie = false,
                ManagerId = this.ManagerId
            });
            this.Players.Add(new LanguageablePlayer_Mockup(this)
            {
                Id = ++playerId, //2
                Name = "Regular",
                Lastname = "Player " + playerId,
                IsRegular = true,
                IsGoalie = false,
                ManagerId = this.ManagerId
            });
            this.Players.Add(new LanguageablePlayer_Mockup(this)
            {
                Id = ++playerId, //3
                Name = "Another Manager",
                Lastname = "Player " + playerId,
                IsRegular = true,
                IsGoalie = false,
                ManagerId = this.ManagerId.HasValue ? (this.ManagerId.Value + 10) : (int?)null
            });
            this.Players.Add(new LanguageablePlayer_Mockup(this)
            {
                Id = ++playerId, //4
                Name = "Regular",
                Lastname = "Player " + playerId,
                IsRegular = true,
                IsGoalie = true,
                ManagerId = this.ManagerId
            });
            this.Players.Add(new LanguageablePlayer_Mockup(this)
            {
                Id = ++playerId, //5
                Name = "Regular",
                Lastname = "Goalie " + playerId,
                IsRegular = true,
                IsGoalie = true,
                ManagerId = this.ManagerId
            });
            this.Players.Add(new LanguageablePlayer_Mockup(this)
            {
                Id = ++playerId, //6
                Name = "Spare",
                Lastname = "Player " + playerId,
                IsRegular = false,
                IsGoalie = true,
                ManagerId = this.ManagerId
            });
            this.Players.Add(new LanguageablePlayer_Mockup(this)
            {
                Id = ++playerId, //7
                Name = "Spare",
                Lastname = "Goalie " + playerId,
                IsRegular = false,
                IsGoalie = true,
                ManagerId = this.ManagerId
            });
            #endregion

        }
    }

    internal class GameConfirmations_Mockup
    {
        public int Game_Id { get; set; }
        public int Player_Id { get; set; }
    }

    internal class LanguageablePlayer_Mockup : LanguageableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string FullName
        {
            get
            {
                return $"{Name} {Lastname}";
            }
        }

        public bool IsRegular { get; set; }
        public bool IsSpare { get { return !IsRegular; } }
        public bool IsGoalie { get; set; }
        public bool IsPlayer { get { return !IsGoalie; } }

        public override string GetSelfValue(string attribute)
        {
            switch (attribute)
            {
                case "Name":
                case "":
                    return this.FullName;
                default:
                    throw new ApplicationException("Not supported SELF attribute for entity Player. Attribute: " + attribute);
            }
        }

        public LanguageableDataModelContext_Mockup Context { get; set; }

        private LanguageableDataModelContext_Mockup _context;

        public LanguageablePlayer_Mockup(LanguageableDataModelContext_Mockup context)
        {
            this._context = context;
        }

        public override bool FilteredEntity(string attribute)
        {
            if (string.IsNullOrWhiteSpace(attribute))
            {
                return true;
            }

            switch (attribute)
            {
                case "Regular":
                    return this.IsRegular;
                case "Spare":
                    return this.IsSpare;
                case "Goalie":
                    return this.IsGoalie;
                case "Skater":
                    return this.IsPlayer;
                case "Confirmed":
                    return this.IsConfirmedForGame();
                case "ConfirmedRegular":
                    return this.IsConfirmedForGame() && this.IsRegular;
                case "UnConfirmedRegular":
                    return !this.IsConfirmedForGame() && this.IsRegular;
                case "UnConfirmedSpare":
                    return !this.IsConfirmedForGame() && this.IsSpare;
                default:
                    throw new ApplicationException("Not supported Level filtering attribute for entity Player. Attribute: " + attribute);
            }
        }


        private bool IsConfirmedForGame()
        {
            var gameExists = this.ParentLevelEntityId.HasValue && this._context.Games.Any(g => g.Id == this.ParentLevelEntityId.Value);

            if (!gameExists)
            {
                throw new ApplicationException($"While Verifying confirmation for player: {this.FullName}, the game against which to verify was not supplied!");
            }

            return this._context.GameConfirmations.Any(gc => gc.Game_Id == this.ParentLevelEntityId.Value && gc.Player_Id == this.Id);
        }
    }

    internal class LanguageableGame_Mockup : LanguageableEntity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        private LanguageableDataModelContext_Mockup _context;

        public LanguageableGame_Mockup(LanguageableDataModelContext_Mockup context)
        {
            this._context = context;
        }

        public override string GetSelfValue(string attribute)
        {
            switch (attribute)
            {
                case "Date":
                    return this.Date.ToLongDateString();
                case "Time":
                    return this.Date.ToShortTimeString();
                case "Full":
                case "":
                    return this.Date.ToString();
                default:
                    throw new ApplicationException("Not supported SELF attribute for entity Game. Attribute: " + attribute);
            }
        }

        public override IEnumerable<ILanguagable> GetPlayers()
        {
            var players =
                from gc in this._context.GameConfirmations
                join p in this._context.Players on gc.Player_Id equals p.Id
                where gc.Game_Id == this.Id
                select p;

            return players.ToList();
        }

        public override bool FilteredEntity(string attribute)
        {
            if (string.IsNullOrWhiteSpace(attribute))
            {
                return true;
            }

            var orderedGames = this.ManagerId.HasValue ?
                _context.Games.Where(g => g.ManagerId == this.ManagerId.Value).OrderBy(g => g.Date) :
                _context.Games.OrderBy(g => g.Date);

            switch (attribute)
            {
                case "Next":
                    var nextGame = orderedGames.FirstOrDefault(g => g.Date > DateTime.Now);
                    return nextGame != null && nextGame.Id == this.Id;
                case "Previous":
                    var previousGame = orderedGames.LastOrDefault(g => g.Date < DateTime.Now);
                    return previousGame != null && previousGame.Id == this.Id;
                case "AfterNext":
                    var nextTwoGames = orderedGames.Where(g => g.Date > DateTime.Now).Take(2);
                    return nextTwoGames.Any(nextG => nextG.Id == this.Id);
                default:
                    throw new ApplicationException("Not supported Level filtering attribute for entity Game. Attribute: " + attribute);
            }
        }
    }


}
