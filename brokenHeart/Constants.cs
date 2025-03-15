using brokenHeart.Authentication.DB;
using brokenHeart.Authentication.Entities;
using brokenHeart.Database.DAO;
using brokenHeart.Database.DAO.Characters;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.Modifiers.Effects.Injuries;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.DAO.Stats;
using brokenHeart.DB;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart
{
    public static class Constants
    {
        public static class Stats
        {
            public static readonly Stat Str = new Stat() { Id = 1, Name = "Strength" };
            public static readonly Stat Dex = new Stat() { Id = 2, Name = "Dexterity" };
            public static readonly Stat Con = new Stat() { Id = 3, Name = "Constitution" };
            public static readonly Stat Int = new Stat() { Id = 4, Name = "Intelligence" };
            public static readonly Stat Ins = new Stat() { Id = 5, Name = "Instincts" };
            public static readonly Stat Cha = new Stat() { Id = 6, Name = "Charisma" };

            public static readonly List<Stat> stats = new() { Str, Dex, Con, Int, Ins, Cha };
        }

        public static class Bodyparts
        {
            public static readonly Bodypart Head = new Bodypart() { Id = 1, Name = "Head" };
            public static readonly Bodypart Torso = new Bodypart() { Id = 2, Name = "Torso" };
            public static readonly Bodypart ArmR = new Bodypart() { Id = 3, Name = "Right Arm" };
            public static readonly Bodypart ArmL = new Bodypart() { Id = 4, Name = "Left Arm" };
            public static readonly Bodypart LegR = new Bodypart() { Id = 5, Name = "Right Leg" };
            public static readonly Bodypart LegL = new Bodypart() { Id = 6, Name = "Left Leg" };
            public static readonly List<Bodypart> BaseBodyparts = new List<Bodypart>()
            {
                Head,
                Torso,
                ArmR,
                ArmL,
                LegR,
                LegL
            };

            //Injuries
            public static readonly InjuryEffectTemplate HeadMinor = new InjuryEffectTemplate()
            {
                Name = "Minor Injury: Head",
                Abstract = "Subtract 1 from ability checks and attack rolls.",
                Duration = "Until treated",
                BodypartId = Head.Id,
                InjuryLevel = InjuryLevel.Minor,
                Description =
                    "Minor damage to your head leaves you slightly confused and off balance.",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "-1 on ability checks and attack rolls"
                }
            };
            public static readonly InjuryEffectTemplate HeadMedium = new InjuryEffectTemplate()
            {
                Name = "Minor Injury: Medium",
                Abstract =
                    "Roll and subtract 1d4 from Dexterity, Intelligence and Instinct checks.",
                Duration = "Until treated",
                BodypartId = Head.Id,
                InjuryLevel = InjuryLevel.Medium,
                Description =
                    "A slight concussion causes you to be unable to properly focus on motor skills and thoughts while your reaction time decreases.",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "-1d4 on DEX, INT and INS"
                }
            };
            public static readonly InjuryEffectTemplate HeadMajor = new InjuryEffectTemplate()
            {
                Name = "Major Injury: Head",
                Abstract =
                    "You fall prone, and roll a CON check DC 10 at the end of every turn, falling prone again on failure.",
                Duration = "Until treated",
                BodypartId = Head.Id,
                InjuryLevel = InjuryLevel.Major,
                Description = "Major head trauma throws you way off balance.",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "Roll CON 10 or fall prone at the end of every turn"
                }
            };

            public static readonly InjuryEffectTemplate TorsoMinor = new InjuryEffectTemplate()
            {
                Name = "Minor Injury: Torso",
                Abstract = "Extensive movement requires a CON check DC 10.",
                Duration = "Until treated",
                BodypartId = Torso.Id,
                InjuryLevel = InjuryLevel.Minor,
                Description =
                    "Minor damage to your torso area causes you to require great effort to perform extensive actions, both physically and mentally.",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "CON check 10 for extensive movement"
                }
            };
            public static readonly InjuryEffectTemplate TorsoMedium = new InjuryEffectTemplate()
            {
                Name = "Medium Injury: Torso",
                Abstract = "Extensive movement will deal 2d4 of damage",
                Duration = "Until treated",
                BodypartId = Torso.Id,
                InjuryLevel = InjuryLevel.Medium,
                Description = "Stretching will cause already present injuries to rip even further.",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "Extensive movement deals 2d4 damage"
                }
            };
            public static readonly InjuryEffectTemplate TorsoMajor = new InjuryEffectTemplate()
            {
                Name = "Major Injury: Torso",
                Abstract = "Extensive movement becomes impossible",
                Duration = "Until treated",
                BodypartId = Torso.Id,
                InjuryLevel = InjuryLevel.Major,
                Description =
                    "Due to massive pain and damage around your torso you are now unable to perform extensive movement, especially without severely worsening your medical situation.",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "No extensive movement"
                }
            };
            public static readonly InjuryEffectTemplate TorsoDismember = new InjuryEffectTemplate()
            {
                Name = "Dismemberment Injury: Torso",
                Abstract =
                    "You take 2d8 of damage at the end of every turn while the wound is not cared for.",
                Duration = "Until treated",
                BodypartId = Torso.Id,
                InjuryLevel = InjuryLevel.Dismember,
                Description =
                    "Severe damage both internal and external has massive effects on your health, continuously dealing further damage",
                Hp = "-2d8",
                RoundReminderTemplate = new RoundReminderTemplate() { Reminder = "Heavy Bleeding" }
            };

            public static readonly InjuryEffectTemplate LeftLegMinor = new InjuryEffectTemplate()
            {
                Name = "Minor Injury: Left Leg",
                Abstract =
                    "Subtract 2 from ability checks when using the affected leg. Dashing requires a CON check DC 10, failure results in halving dash distance.",
                Duration = "Until treated",
                BodypartId = LegL.Id,
                InjuryLevel = InjuryLevel.Minor,
                Description =
                    "The pain from a light injury or the effects of having been hit in an unfortunate spot makes straining actions difficult.",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "-2 on ability checks on this leg, CON check 10 for dash"
                }
            };
            public static readonly InjuryEffectTemplate LeftLegMedium = new InjuryEffectTemplate()
            {
                Name = "Medium Injury: Left Leg",
                Abstract =
                    "Actions reliant on stability require a DEX check DC 10. Moving more than half your movement speed requires a CON check DC 10.",
                Duration = "Until treated",
                BodypartId = LegL.Id,
                InjuryLevel = InjuryLevel.Medium,
                Description =
                    "Large cuts or a chipped bone throw you slightly off balance and make heavy impact severly uncomfortable.",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder =
                        "DEX check 10 for unstable actions, CON check 10 for moving more than half your speed"
                }
            };
            public static readonly InjuryEffectTemplate LeftLegMajor = new InjuryEffectTemplate()
            {
                Name = "Major Injury: Left Leg",
                Abstract =
                    "Your movement speed is halved. A major injury on the other leg halves your speed again.",
                Duration = "Until treated",
                BodypartId = LegL.Id,
                InjuryLevel = InjuryLevel.Major,
                Description =
                    "A broken bone or profuse bleeding leaves your legs weak and unstable, making even walking difficult.",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "Half movement speed per leg"
                }
            };
            public static readonly InjuryEffectTemplate LeftLegDismember =
                new InjuryEffectTemplate()
                {
                    Name = "Dismemberment Injury: Left Leg",
                    Abstract =
                        "You lose the affected leg. You take 1d10 of damage at the end of every turn while the wound is not cared for.",
                    Duration = "Until treated",
                    BodypartId = LegL.Id,
                    InjuryLevel = InjuryLevel.Dismember,
                    Description =
                        "Your leg is battered beyond saving or removed outright. Additionally, the heavy damage sustained now causes further continuous damage.",
                    Hp = "-1d10",
                    RoundReminderTemplate = new RoundReminderTemplate()
                    {
                        Reminder = "Missing affected leg, heavy bleeding"
                    }
                };

            public static readonly InjuryEffectTemplate RightLegMinor = new InjuryEffectTemplate()
            {
                Name = "Minor Injury: Right Leg",
                Abstract =
                    "Subtract 2 from ability checks when using the affected leg. Dashing requires a CON check DC 10, failure results in halving dash distance.",
                Duration = "Until treated",
                BodypartId = LegR.Id,
                InjuryLevel = InjuryLevel.Minor,
                Description =
                    "The pain from a light injury or the effects of having been hit in an unfortunate spot makes straining actions difficult.",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "-2 on ability checks on this leg, CON check 10 for dash"
                }
            };
            public static readonly InjuryEffectTemplate RightLegMedium = new InjuryEffectTemplate()
            {
                Name = "Medium Injury: Right Leg",
                Abstract =
                    "Actions reliant on stability require a DEX check DC 10. Moving more than half your movement speed requires a CON check DC 10.",
                Duration = "Until treated",
                BodypartId = LegR.Id,
                InjuryLevel = InjuryLevel.Medium,
                Description =
                    "Large cuts or a chipped bone throw you slightly off balance and make heavy impact severly uncomfortable.",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder =
                        "DEX check 10 for unstable actions, CON check 10 for moving more than half your speed"
                }
            };
            public static readonly InjuryEffectTemplate RightLegMajor = new InjuryEffectTemplate()
            {
                Name = "Major Injury: Right Leg",
                Abstract =
                    "Your movement speed is halved. A major injury on the other leg halves your speed again.",
                Duration = "Until treated",
                BodypartId = LegR.Id,
                InjuryLevel = InjuryLevel.Major,
                Description =
                    "A broken bone or profuse bleeding leaves your legs weak and unstable, making even walking difficult.",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "Half movement speed per leg"
                }
            };
            public static readonly InjuryEffectTemplate RightLegDismember =
                new InjuryEffectTemplate()
                {
                    Name = "Dismemberment Injury: Right Leg",
                    Abstract =
                        "You lose the affected leg. You take 1d10 of damage at the end of every turn while the wound is not cared for.",
                    Duration = "Until treated",
                    BodypartId = LegR.Id,
                    InjuryLevel = InjuryLevel.Dismember,
                    Description =
                        "Your leg is battered beyond saving or removed outright. Additionally, the heavy damage sustained now causes further continuous damage.",
                    Hp = "-1d10",
                    RoundReminderTemplate = new RoundReminderTemplate()
                    {
                        Reminder = "Missing affected leg, heavy bleeding"
                    }
                };

            public static readonly InjuryEffectTemplate LeftArmMinor = new InjuryEffectTemplate()
            {
                Name = "Minor Injury: Left Arm",
                Abstract = "Subtract 2 from ability checks using the affected arm.",
                Duration = "Until treated",
                BodypartId = ArmL.Id,
                InjuryLevel = InjuryLevel.Minor,
                Description =
                    "You have received a bad bruise, a cut or a similar injury. The pain this causes or the location of the injury prevents you from using your arm at full strength.",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "-2 on ability checks on this arm."
                }
            };
            public static readonly InjuryEffectTemplate LeftArmMedium = new InjuryEffectTemplate()
            {
                Name = "Medium Injury: Left Arm",
                Abstract = "Straining actions using the affected arm require a CON check DC 10.",
                Duration = "Until treated",
                BodypartId = ArmL.Id,
                InjuryLevel = InjuryLevel.Medium,
                Description =
                    "Pain and tissue damage causes you difficulty to complete actions heavily straining on the injured arm",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "Straining actions on this arm require CON check 10"
                }
            };
            public static readonly InjuryEffectTemplate LeftArmMajor = new InjuryEffectTemplate()
            {
                Name = "Major Injury: Left Arm",
                Abstract = "You cannot use the affected arm to perform straining actions.",
                Duration = "Until treated",
                BodypartId = ArmL.Id,
                InjuryLevel = InjuryLevel.Major,
                Description =
                    "Deep and heavy damage such as a broken bone prevents you from properly using your arm at all, especially without worsening the damage yourself.",

                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "Straining actions on this arm are impossible"
                }
            };
            public static readonly InjuryEffectTemplate LeftArmDismember =
                new InjuryEffectTemplate()
                {
                    Name = "Dismemberment Injury: Left Arm",
                    Abstract =
                        "You lose your arm. You also take 1d10 of damage at the end of every turn while the wound is not cared for.",
                    Duration = "Until treated",
                    BodypartId = ArmL.Id,
                    InjuryLevel = InjuryLevel.Dismember,
                    Description =
                        "Your Arm is lost or damaged beyond healing, with extensive bleeding causing continuous health damage",
                    Hp = "-1d10",

                    RoundReminderTemplate = new RoundReminderTemplate()
                    {
                        Reminder = "Missing affected arm, heavily bleeding"
                    }
                };

            public static readonly InjuryEffectTemplate RightArmMinor = new InjuryEffectTemplate()
            {
                Name = "Minor Injury: Right Arm",
                Abstract = "Subtract 2 from ability checks using the affected arm.",
                Duration = "Until treated",
                BodypartId = ArmR.Id,
                InjuryLevel = InjuryLevel.Minor,
                Description =
                    "You have received a bad bruise, a cut or a similar injury. The pain this causes or the location of the injury prevents you from using your arm at full strength.",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "-2 on ability checks on this arm."
                }
            };
            public static readonly InjuryEffectTemplate RightArmMedium = new InjuryEffectTemplate()
            {
                Name = "Medium Injury: Right Arm",
                Abstract = "Straining actions using the affected arm require a CON check DC 10.",
                Duration = "Until treated",
                BodypartId = ArmR.Id,
                InjuryLevel = InjuryLevel.Medium,
                Description =
                    "Pain and tissue damage causes you difficulty to complete actions heavily straining on the injured arm",
                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "Straining actions on this arm require CON check 10"
                }
            };
            public static readonly InjuryEffectTemplate RightArmMajor = new InjuryEffectTemplate()
            {
                Name = "Major Injury: Right Arm",
                Abstract = "You cannot use the affected arm to perform straining actions.",
                Duration = "Until treated",
                BodypartId = ArmR.Id,
                InjuryLevel = InjuryLevel.Major,
                Description =
                    "Deep and heavy damage such as a broken bone prevents you from properly using your arm at all, especially without worsening the damage yourself.",

                RoundReminderTemplate = new RoundReminderTemplate()
                {
                    Reminder = "Straining actions on this arm are impossible"
                }
            };
            public static readonly InjuryEffectTemplate RightArmDismember =
                new InjuryEffectTemplate()
                {
                    Name = "Dismemberment Injury: Right Arm",
                    Abstract =
                        "You lose your arm. You also take 1d10 of damage at the end of every turn while the wound is not cared for.",
                    Duration = "Until treated",
                    BodypartId = ArmR.Id,
                    InjuryLevel = InjuryLevel.Dismember,
                    Description =
                        "Your Arm is lost or damaged beyond healing, with extensive bleeding causing continuous health damage",
                    Hp = "-1d10",

                    RoundReminderTemplate = new RoundReminderTemplate()
                    {
                        Reminder = "Missing affected arm, heavily bleeding"
                    }
                };

            public static readonly List<InjuryEffectTemplate> InjuryEffects =
                new List<InjuryEffectTemplate>()
                {
                    HeadMinor,
                    HeadMedium,
                    HeadMajor,
                    TorsoMinor,
                    TorsoMedium,
                    TorsoMajor,
                    TorsoDismember,
                    LeftLegMinor,
                    LeftLegMedium,
                    LeftLegMajor,
                    LeftLegDismember,
                    RightLegMinor,
                    RightLegMedium,
                    RightLegMajor,
                    RightLegDismember,
                    LeftArmMinor,
                    LeftArmMedium,
                    LeftArmMajor,
                    LeftArmDismember,
                    RightArmMinor,
                    RightArmMedium,
                    RightArmMajor,
                    RightArmDismember
                };
        }

        public static readonly CounterTemplate Dying = new CounterTemplate()
        {
            Name = "Dying",
            Max = 3,
            Description = "This counter indicates the number of rounds you are away from dying.",
            RoundBased = false
        };

        public static async Task ValidateAsync(
            BrokenDbContext _dbContext,
            AuthDbContext _authContext
        )
        {
            //Validate Stats
            List<Stat> dbMainStats = await _dbContext.Stats.ToListAsync();
            foreach (Stat mainStat in Stats.stats)
            {
                if (!dbMainStats.Select(x => x.Name).Contains(mainStat.Name))
                {
                    _dbContext.Stats.Add(mainStat);
                }
            }

            //Validate Bodyparts
            List<Bodypart> dbBodyparts = await _dbContext.Bodyparts.ToListAsync();
            foreach (Bodypart bodypart in Bodyparts.BaseBodyparts)
            {
                if (!dbBodyparts.Select(x => x.Name).Contains(bodypart.Name))
                {
                    _dbContext.Bodyparts.Add(bodypart);
                }
            }

            //Validate premade Injuries
            List<InjuryEffectTemplate> dbInjuryEffectTemplate =
                await _dbContext.InjuryEffectTemplates.ToListAsync();
            foreach (InjuryEffectTemplate injuryEffectTemplate in Bodyparts.InjuryEffects)
            {
                if (!dbInjuryEffectTemplate.Select(x => x.Name).Contains(injuryEffectTemplate.Name))
                {
                    _dbContext.EffectTemplates.Add(injuryEffectTemplate);
                }
            }

            foreach (User user in _authContext.Users)
            {
                if (
                    !(_dbContext.UserSimplified.Where(x => x.Username == user.Username).Count() > 0)
                )
                {
                    _dbContext.UserSimplified.Add(
                        new UserSimplified()
                        {
                            Username = user.Username,
                            DiscordId = user.DiscordId
                        }
                    );
                }
            }

            _dbContext.SaveChanges();
        }
    }
}
