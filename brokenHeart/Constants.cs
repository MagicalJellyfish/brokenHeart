using brokenHeart.Entities.Characters;
using brokenHeart.Entities.Effects;
using brokenHeart.Entities.Counters;
using brokenHeart.DB;
using Microsoft.EntityFrameworkCore;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;
using brokenHeart.Auth.DB;
using brokenHeart.Auth;

namespace brokenHeart
{
    public static class Constants
    {
        public static class Stats
        {
            public static readonly Stat Str = new Stat(1, "Strength");
            public static readonly Stat Dex = new Stat(2, "Dexterity");
            public static readonly Stat Con = new Stat(3, "Constitution");
            public static readonly Stat Int = new Stat(4, "Intelligence");
            public static readonly Stat Ins = new Stat(5, "Instincts");
            public static readonly Stat Cha = new Stat(6, "Charisma");

            public static readonly List<Stat> stats = new() { Str, Dex, Con, Int, Ins, Cha };
        }

        public static class Bodyparts
        {
            public static readonly Bodypart Head = new Bodypart(1, "Head");
            public static readonly Bodypart Torso = new Bodypart(2, "Torso");
            public static readonly Bodypart ArmR = new Bodypart(3, "Right Arm");
            public static readonly Bodypart ArmL = new Bodypart(4, "Left Arm");
            public static readonly Bodypart LegR = new Bodypart(5, "Right Leg");
            public static readonly Bodypart LegL = new Bodypart(6, "Left Leg");
            public static readonly List<Bodypart> BaseBodyparts = new List<Bodypart>() { Head, Torso, ArmR, ArmL, LegR, LegL };

            //Injuries
            public static readonly EffectTemplate HeadMinor = new EffectTemplate("Minor Head Injury",
                "Subtract 1 from ability checks or saves.", "Until treated",
                "Minor damage to your head leave you slightly confused and off balance.",
                reminderTemplate: new RoundReminderTemplate("-1 on ability checks & saves"));
            public static readonly EffectTemplate HeadMedium = new EffectTemplate("Medium Head Injury",
                "Roll and subtract 1d4 from Dexterity, Intelligence and Instinct checks and saves.", "Until treated",
                "A slight concussion causes you to be unable to properly focus on motor skills and thoughts while your reaction time increases.",
                reminderTemplate: new RoundReminderTemplate("-1d4 on DEX, INT and INS"));
            public static readonly EffectTemplate HeadMajor = new EffectTemplate("Major Head Injury",
                "You fall prone, and roll a perseverance check at the end of every turn. On 10 or less, you fall prone again.", "Until treated",
                "Major head trauma throws you way off balance.",
                reminderTemplate: new RoundReminderTemplate("Roll perseverance 10 or fall prone at the end of every turn"));

            public static readonly EffectTemplate TorsoMinor = new EffectTemplate("Minor Torso Injury",
                "Extensive movement requires a Perseverance check.", "Until treated",
                "Minor damage to your torso area cause you to require great effort to perform extensive actions, both physically and mentally.",
                reminderTemplate: new RoundReminderTemplate("Perseverance check for extensive movement"));
            public static readonly EffectTemplate TorsoMedium = new EffectTemplate("Medium Torso Injury",
                "Extensive movement will deal 2d4 of damage", "Until treated",
                "Stretching too much will cause already present injuries to rip even further.",
                 reminderTemplate: new RoundReminderTemplate("Extensive movement deals 2d4 damage"));
            public static readonly EffectTemplate TorsoMajor = new EffectTemplate("Major Torso Injury",
                "Extensive movement becomes impossible", "Until treated",
                "Due to massive pain and damage around your torso you are now unable to perform extensive movement, " +
                "especially without severely worsening your medical situation.",
                reminderTemplate: new RoundReminderTemplate("No extensive movement"));
            public static readonly EffectTemplate TorsoDismember = new EffectTemplate("Dismemberment Torso Injury",
                "You take 2d8 of damage at the end of every turn while the wound is not cared for.", "Until treated",
                "Severe damage both internal and external has massive effects on your health, continuously dealing further damage",
                 reminderTemplate: new RoundReminderTemplate("Heavy Bleeding"), hp: "-2d8");

            public static readonly EffectTemplate LegMinor = new EffectTemplate("Minor Leg Injury",
                "Subtract 2 from ability checks or saves when using the affected leg. Dashing requires a perseverance check, " +
                "failure results in halving dash distance.", "Until treated",
                "The pain from a light injury or the effects of having been hit in an unfortunate spot makes straining actions difficult.",
                reminderTemplate: new RoundReminderTemplate("-2 on ability checks & saves on this leg, perseverance check for dash"));
            public static readonly EffectTemplate LegMedium = new EffectTemplate("Medium Leg Injury",
                "Actions reliant on stability require a balance check. Moving more than half your movement speed requires a perseverance check.", "Until treated",
                "Large cuts or a chipped bone throw you slightly off balance and make heavy impact severly uncomfortable.",
                reminderTemplate: new RoundReminderTemplate("Balance check for unstable actions, moving more than half your speed requires a Perseverance check"));
            public static readonly EffectTemplate LegMajor = new EffectTemplate("Major Leg Injury",
                "Your movement speed is halved. A major injury on the other leg halves your speed again.", "Until treated",
                "A broken bone or profuse bleeding leaves your legs weak and unstable, making even walking difficult. " +
                "With two majorly affected legs you might as well crawl.",
                reminderTemplate: new RoundReminderTemplate("Half movement speed per leg"));
            public static readonly EffectTemplate LegDismember = new EffectTemplate("Dismemberment Leg Injury",
                "You lose the affected leg. Usually your best movement option is to crawl. " +
                "You take 1d10 of damage at the end of every turn while the wound is not cared for.", "Until treated",
                "Your leg is battered beyond saving or removed outright. You can try to balance on one leg, " +
                "though your best movement option is crawling at this point. Additionally, the heavy damage sustained now causes further continuous damage.",
                reminderTemplate: new RoundReminderTemplate("Missing affected leg, heavy bleeding"), hp: "-1d10");

            public static readonly EffectTemplate ArmMinor = new EffectTemplate("Minor Arm Injury",
                "Subtract 2 from ability checks or saves using the affected arm.", "Until treated",
                "You have received a bad bruise, a cut or a similar injury. The pain this causes or the location of the injury prevents you from using " +
                "your arm at full strength",
                reminderTemplate: new RoundReminderTemplate("-2 on ability checks & saves on this arm."));
            public static readonly EffectTemplate ArmMedium = new EffectTemplate("Medium Arm Injury",
                "Grappling, heavy pulling/pushing/lifting or similar actions using the affected arm require a Perseverance check.", "Until treated",
                "Pain and tissue damage causes you difficulty to complete actions heavily straining on the injured arm",
                reminderTemplate: new RoundReminderTemplate("Straining actions on this arm require a Perseverance check"));
            public static readonly EffectTemplate ArmMajor = new EffectTemplate("Major Arm Injury",
                "You cannot use the affected arm to perform straining actions.", "Until treated",
                "Deep and heavy damage such as a broken bone prevents you from properly using your arm at all, especially without " +
                "worsening the damage yourself.",
                reminderTemplate: new RoundReminderTemplate("Straining actions on this arm are impossible"));
            public static readonly EffectTemplate ArmDismember = new EffectTemplate("Dismemberment Arm Injury",
                "You lose your arm. You also take 1d10 of damage at the end of every turn while the wound is not cared for.", "Until treated",
                "Your Arm is lost or damaged beyond healing, with extensive bleeding causing continuous health damage", hp: "-1d10",
                reminderTemplate: new RoundReminderTemplate("Missing affected arm, heavily bleeding"));

            public static readonly List<EffectTemplate> InjuryEffects = new List<EffectTemplate>()
            {
                HeadMinor, HeadMedium, HeadMajor,
                TorsoMinor, TorsoMedium, TorsoMajor, TorsoDismember,
                LegMinor, LegMedium, LegMajor, LegDismember,
                ArmMinor, ArmMedium, ArmMajor, ArmDismember
            };
        }

        public static readonly CounterTemplate Dying = new CounterTemplate("Dying", 3,
            "This counter indicates the number of rounds you are away from dying.", true);

        public static async Task ValidateAsync(BrokenDbContext _dbContext, AuthDbContext _authContext)
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
            List<EffectTemplate> dbEffectTemplates = await _dbContext.EffectTemplates.ToListAsync();
            foreach (EffectTemplate effectTemplate in Bodyparts.InjuryEffects)
            {
                if (!dbEffectTemplates.Select(x => x.Name).Contains(effectTemplate.Name))
                {
                    _dbContext.EffectTemplates.Add(effectTemplate);
                }
            }

            //Validate Death Counter
            if (!(await _dbContext.CounterTemplates.Select(x => x.Name).ToListAsync()).Contains(Dying.Name))
            {
                _dbContext.CounterTemplates.Add(Dying);
            }

            foreach (ApplicationUser user in _authContext.Users)
            {
                if (!(_dbContext.UserSimplified.Where(x => x.Username == user.UserName).Count() > 0))
                {
                    _dbContext.UserSimplified.Add(new Entities.UserSimplified(user.UserName, user.DiscordId));
                }
            }

            _dbContext.SaveChanges();
        }
    }
}
