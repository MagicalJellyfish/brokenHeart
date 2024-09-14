using System.Security.Policy;
using brokenHeart.Auth;
using brokenHeart.Auth.DB;
using brokenHeart.DB;
using brokenHeart.Entities;
using brokenHeart.Entities.Characters;
using brokenHeart.Entities.Counters;
using brokenHeart.Entities.Effects;
using brokenHeart.Entities.Effects.Injuries;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
            public static readonly InjuryEffectTemplate HeadMinor = new InjuryEffectTemplate(
                "Minor Injury: Head",
                "Subtract 1 from ability checks and attack rolls.",
                "Until treated",
                Head.Id,
                InjuryLevel.Minor,
                "Minor damage to your head leaves you slightly confused and off balance.",
                reminderTemplate: new RoundReminderTemplate("-1 on ability checks and attack rolls")
            );
            public static readonly InjuryEffectTemplate HeadMedium = new InjuryEffectTemplate(
                "Medium Injury: Head",
                "Roll and subtract 1d4 from Dexterity, Intelligence and Instinct checks.",
                "Until treated",
                Head.Id,
                InjuryLevel.Medium,
                "A slight concussion causes you to be unable to properly focus on motor skills and thoughts while your reaction time decreases.",
                reminderTemplate: new RoundReminderTemplate("-1d4 on DEX, INT and INS")
            );
            public static readonly InjuryEffectTemplate HeadMajor = new InjuryEffectTemplate(
                "Major Injury: Head",
                "You fall prone, and roll an INS check DC 10 at the end of every turn, falling prone again on failure.",
                "Until treated",
                Head.Id,
                InjuryLevel.Major,
                "Major head trauma throws you way off balance.",
                reminderTemplate: new RoundReminderTemplate(
                    "Roll INS 10 or fall prone at the end of every turn"
                )
            );

            public static readonly InjuryEffectTemplate TorsoMinor = new InjuryEffectTemplate(
                "Minor Injury: Torso",
                "Extensive movement requires an INS check DC 10.",
                "Until treated",
                Torso.Id,
                InjuryLevel.Minor,
                "Minor damage to your torso area causes you to require great effort to perform extensive actions, both physically and mentally.",
                reminderTemplate: new RoundReminderTemplate("INS check 10 for extensive movement")
            );
            public static readonly InjuryEffectTemplate TorsoMedium = new InjuryEffectTemplate(
                "Medium Injury: Torso",
                "Extensive movement will deal 2d4 of damage",
                "Until treated",
                Torso.Id,
                InjuryLevel.Medium,
                "Stretching will cause already present injuries to rip even further.",
                reminderTemplate: new RoundReminderTemplate("Extensive movement deals 2d4 damage")
            );
            public static readonly InjuryEffectTemplate TorsoMajor = new InjuryEffectTemplate(
                "Major Injury: Torso",
                "Extensive movement becomes impossible",
                "Until treated",
                Torso.Id,
                InjuryLevel.Major,
                "Due to massive pain and damage around your torso you are now unable to perform extensive movement, "
                    + "especially without severely worsening your medical situation.",
                reminderTemplate: new RoundReminderTemplate("No extensive movement")
            );
            public static readonly InjuryEffectTemplate TorsoDismember = new InjuryEffectTemplate(
                "Dismemberment Injury: Torso",
                "You take 2d8 of damage at the end of every turn while the wound is not cared for.",
                "Until treated",
                Torso.Id,
                InjuryLevel.Dismember,
                "Severe damage both internal and external has massive effects on your health, continuously dealing further damage",
                reminderTemplate: new RoundReminderTemplate("Heavy Bleeding"),
                hp: "-2d8"
            );

            public static readonly InjuryEffectTemplate LeftLegMinor = new InjuryEffectTemplate(
                "Minor Injury: Left Leg",
                "Subtract 2 from ability checks when using the affected leg. Dashing requires an INS check DC 10, "
                    + "failure results in halving dash distance.",
                "Until treated",
                LegL.Id,
                InjuryLevel.Minor,
                "The pain from a light injury or the effects of having been hit in an unfortunate spot makes straining actions difficult.",
                reminderTemplate: new RoundReminderTemplate(
                    "-2 on ability checks on this leg, INS check 10 for dash"
                )
            );
            public static readonly InjuryEffectTemplate LeftLegMedium = new InjuryEffectTemplate(
                "Medium Injury: Left Leg",
                "Actions reliant on stability require a DEX check DC 10. Moving more than half your movement speed requires an INS check DC 10.",
                "Until treated",
                LegL.Id,
                InjuryLevel.Medium,
                "Large cuts or a chipped bone throw you slightly off balance and make heavy impact severly uncomfortable.",
                reminderTemplate: new RoundReminderTemplate(
                    "DEX check 10 for unstable actions, INS check 10 for moving more than half your speed"
                )
            );
            public static readonly InjuryEffectTemplate LeftLegMajor = new InjuryEffectTemplate(
                "Major Injury: Left Leg",
                "Your movement speed is halved. A major injury on the other leg halves your speed again.",
                "Until treated",
                LegL.Id,
                InjuryLevel.Major,
                "A broken bone or profuse bleeding leaves your legs weak and unstable, making even walking difficult.",
                reminderTemplate: new RoundReminderTemplate("Half movement speed per leg")
            );
            public static readonly InjuryEffectTemplate LeftLegDismember = new InjuryEffectTemplate(
                "Dismemberment Injury: Left Leg",
                "You lose the affected leg. You take 1d10 of damage at the end of every turn while the wound is not cared for.",
                "Until treated",
                LegL.Id,
                InjuryLevel.Dismember,
                "Your leg is battered beyond saving or removed outright. Additionally, the heavy damage sustained now causes further continuous damage.",
                reminderTemplate: new RoundReminderTemplate("Missing affected leg, heavy bleeding"),
                hp: "-1d10"
            );

            public static readonly InjuryEffectTemplate RightLegMinor = new InjuryEffectTemplate(
                "Minor Injury: Right Leg",
                "Subtract 2 from ability checks when using the affected leg. Dashing requires an INS check DC 10, "
                    + "failure results in halving dash distance.",
                "Until treated",
                LegR.Id,
                InjuryLevel.Minor,
                "The pain from a light injury or the effects of having been hit in an unfortunate spot makes straining actions difficult.",
                reminderTemplate: new RoundReminderTemplate(
                    "-2 on ability checks on this leg, INS check 10 for dash"
                )
            );
            public static readonly InjuryEffectTemplate RightLegMedium = new InjuryEffectTemplate(
                "Medium Injury: Right Leg",
                "Actions reliant on stability require a DEX check DC 10. Moving more than half your movement speed requires an INS check DC 10.",
                "Until treated",
                LegR.Id,
                InjuryLevel.Medium,
                "Large cuts or a chipped bone throw you slightly off balance and make heavy impact severly uncomfortable.",
                reminderTemplate: new RoundReminderTemplate(
                    "DEX check 10 for unstable actions, INS check 10 for moving more than half your speed"
                )
            );
            public static readonly InjuryEffectTemplate RightLegMajor = new InjuryEffectTemplate(
                "Major Injury: Right Leg",
                "Your movement speed is halved. A major injury on the other leg halves your speed again.",
                "Until treated",
                LegR.Id,
                InjuryLevel.Major,
                "A broken bone or profuse bleeding leaves your legs weak and unstable, making even walking difficult.",
                reminderTemplate: new RoundReminderTemplate("Half movement speed per leg")
            );
            public static readonly InjuryEffectTemplate RightLegDismember =
                new InjuryEffectTemplate(
                    "Dismemberment Injury: Right Leg",
                    "You lose the affected leg. You take 1d10 of damage at the end of every turn while the wound is not cared for.",
                    "Until treated",
                    LegR.Id,
                    InjuryLevel.Dismember,
                    "Your leg is battered beyond saving or removed outright. Additionally, the heavy damage sustained now causes further continuous damage.",
                    reminderTemplate: new RoundReminderTemplate(
                        "Missing affected leg, heavy bleeding"
                    ),
                    hp: "-1d10"
                );

            public static readonly InjuryEffectTemplate LeftArmMinor = new InjuryEffectTemplate(
                "Minor Injury: Left Arm",
                "Subtract 2 from ability checks using the affected arm.",
                "Until treated",
                ArmL.Id,
                InjuryLevel.Minor,
                "You have received a bad bruise, a cut or a similar injury. The pain this causes or the location of the injury prevents you from using "
                    + "your arm at full strength.",
                reminderTemplate: new RoundReminderTemplate("-2 on ability checks on this arm.")
            );
            public static readonly InjuryEffectTemplate LeftArmMedium = new InjuryEffectTemplate(
                "Medium Injury: Left Arm",
                "Straining actions using the affected arm require an INS check DC 10.",
                "Until treated",
                ArmL.Id,
                InjuryLevel.Medium,
                "Pain and tissue damage causes you difficulty to complete actions heavily straining on the injured arm",
                reminderTemplate: new RoundReminderTemplate(
                    "Straining actions on this arm require INS check 10"
                )
            );
            public static readonly InjuryEffectTemplate LeftArmMajor = new InjuryEffectTemplate(
                "Major Injury: Left Arm",
                "You cannot use the affected arm to perform straining actions.",
                "Until treated",
                ArmL.Id,
                InjuryLevel.Major,
                "Deep and heavy damage such as a broken bone prevents you from properly using your arm at all, especially without "
                    + "worsening the damage yourself.",
                reminderTemplate: new RoundReminderTemplate(
                    "Straining actions on this arm are impossible"
                )
            );
            public static readonly InjuryEffectTemplate LeftArmDismember = new InjuryEffectTemplate(
                "Dismemberment Injury: Left Arm",
                "You lose your arm. You also take 1d10 of damage at the end of every turn while the wound is not cared for.",
                "Until treated",
                ArmL.Id,
                InjuryLevel.Dismember,
                "Your Arm is lost or damaged beyond healing, with extensive bleeding causing continuous health damage",
                hp: "-1d10",
                reminderTemplate: new RoundReminderTemplate(
                    "Missing affected arm, heavily bleeding"
                )
            );

            public static readonly InjuryEffectTemplate RightArmMinor = new InjuryEffectTemplate(
                "Minor Injury: Right Arm",
                "Subtract 2 from ability checks using the affected arm.",
                "Until treated",
                ArmR.Id,
                InjuryLevel.Minor,
                "You have received a bad bruise, a cut or a similar injury. The pain this causes or the location of the injury prevents you from using "
                    + "your arm at full strength.",
                reminderTemplate: new RoundReminderTemplate("-2 on ability checks on this arm.")
            );
            public static readonly InjuryEffectTemplate RightArmMedium = new InjuryEffectTemplate(
                "Medium Injury: Right Arm",
                "Straining actions using the affected arm require an INS check DC 10.",
                "Until treated",
                ArmR.Id,
                InjuryLevel.Medium,
                "Pain and tissue damage causes you difficulty to complete actions heavily straining on the injured arm",
                reminderTemplate: new RoundReminderTemplate(
                    "Straining actions on this arm require INS check 10"
                )
            );
            public static readonly InjuryEffectTemplate RightArmMajor = new InjuryEffectTemplate(
                "Major Injury: Right Arm",
                "You cannot use the affected arm to perform straining actions.",
                "Until treated",
                ArmR.Id,
                InjuryLevel.Major,
                "Deep and heavy damage such as a broken bone prevents you from properly using your arm at all, especially without "
                    + "worsening the damage yourself.",
                reminderTemplate: new RoundReminderTemplate(
                    "Straining actions on this arm are impossible"
                )
            );
            public static readonly InjuryEffectTemplate RightArmDismember =
                new InjuryEffectTemplate(
                    "Dismemberment Injury: Right Arm",
                    "You lose your arm. You also take 1d10 of damage at the end of every turn while the wound is not cared for.",
                    "Until treated",
                    ArmR.Id,
                    InjuryLevel.Dismember,
                    "Your Arm is lost or damaged beyond healing, with extensive bleeding causing continuous health damage",
                    hp: "-1d10",
                    reminderTemplate: new RoundReminderTemplate(
                        "Missing affected arm, heavily bleeding"
                    )
                );

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

        public static readonly CounterTemplate Dying = new CounterTemplate(
            "Dying",
            3,
            "This counter indicates the number of rounds you are away from dying.",
            false
        );

        public static async Task ValidateAsync(
            BrokenDbContext _dbContext,
            AuthDbContext _authContext,
            RoleManager<IdentityRole> roleManager
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

            //Validate Death Counter
            if (
                !(await _dbContext.CounterTemplates.Select(x => x.Name).ToListAsync()).Contains(
                    Dying.Name
                )
            )
            {
                _dbContext.CounterTemplates.Add(Dying);
            }

            foreach (ApplicationUser user in _authContext.Users)
            {
                if (
                    !(_dbContext.UserSimplified.Where(x => x.Username == user.UserName).Count() > 0)
                )
                {
                    _dbContext.UserSimplified.Add(
                        new Entities.UserSimplified(user.UserName, user.DiscordId)
                    );
                }
            }

            List<string> roles = new List<string>() { UserRoles.User, UserRoles.Admin };
            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            _dbContext.SaveChanges();
        }
    }
}
