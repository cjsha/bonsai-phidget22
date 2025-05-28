using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Phidget22;
using Phidget22.Events;

namespace Cjsha.Phidget22
{
    [Description("")]
    [Combinator(MethodName = nameof(Generate))]
    [WorkflowElementCategory(ElementCategory.Source)]
    public class RotaryEncoderEventDriven
    {
        [Description("")]
        [Range(1, 1000)]
        public int PositionChangeTrigger { get; set; } = 1;

        [Description("")]
        [Range(0, 5)]
        public int HubPort { get; set; } = 0;

        public IObservable<long> Generate()
        {
            Encoder encoder = new Encoder{ HubPort = HubPort };
            encoder.Open(Phidget.DefaultTimeout);
            encoder.PositionChangeTrigger = PositionChangeTrigger;

            return Observable.FromEventPattern<EncoderPositionChangeEventHandler, EncoderPositionChangeEventArgs>
            (
                h => encoder.PositionChange += h,
                h => encoder.PositionChange -= h
            )
            .Select(encoderEvent => ((Encoder)encoderEvent.Sender).Position)
            .Finally(() => encoder.Close());
        }
    }
}
