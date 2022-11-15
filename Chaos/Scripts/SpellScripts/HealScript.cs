using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Formulae;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.SpellScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.SpellScripts
{
    public class HealScript : ConfigurableSpellScriptBase
    {
        protected Animation? Animation { get; init; }
        protected ushort AnimationSpeed { get; init; } = 100;
        protected BodyAnimation? BodyAnimation { get; init; }
        protected int? HealAmount { get; init; }
        protected int? HealPercent { get; init; }
        protected int? manaSpent { get; init; }
        protected byte? Sound { get; init; }
        protected ushort? SourceAnimation { get; init; }
        protected ushort? TargetAnimation { get; init; }

        public HealScript(Spell subject) : base(subject)
        {
            if (SourceAnimation.HasValue || TargetAnimation.HasValue)
                Animation = new Animation
                {
                    AnimationSpeed = AnimationSpeed,
                    SourceAnimation = SourceAnimation ?? 0,
                    TargetAnimation = TargetAnimation ?? 0
                };
        }

        public override void OnUse(SpellContext context)
        {
            var source = context.Source;
            var target = context.Target;
            var map = source.MapInstance;

            //Check for mana and send a message if incorrect or insuffcient mana
            if (source.StatSheet.CurrentMp < manaSpent!.Value)
            {
                source.SendServerMessage(ServerMessageType.OrangeBar1, "Your will is too weak.", source.Id);
                return;
            }

            if (BodyAnimation.HasValue)
                source.AnimateBody(BodyAnimation.Value);

            if (Sound.HasValue)
                map.PlaySound(Sound.Value, target);

            if (Animation != null)
                target.Animate(Animation, source.Id);

            //Let's take mana away from the caster
            source.StatSheet.SubtractMp(manaSpent!.Value);
            source.SendAttributes(source.Id);

            var amount = HealAmount ?? (source.StatSheet.MaximumHp / 100) * HealPercent;
            var amountNew = amount + (source.StatSheet.Wis * 2);

            //Let's add HP to the person casted on and send them a message they have been healed
            target.StatSheet.AddHp(amountNew!.Value);
            target.SendServerMessage(ServerMessageType.OrangeBar1, $"{source.Name} has healed you for {amountNew!.Value} using {Subject.Template.Name}.", target.Id);
            target.SendAttributes(target.Id);
        }
    }
}
