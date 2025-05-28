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
    public class RotaryEncoderPolling
    {
        [Description("")]
        public float SampleFrequencyHz { get; set; } = 100;

        [Description("")]
        [Range(0, 5)]
        public int Channel { get; set; } = 0;

        public IObservable<long> Generate()
        {
            Encoder encoder = new Encoder { Channel = Channel };
            encoder.Open(Phidget.DefaultTimeout);
            encoder.DataRate = SampleFrequencyHz;

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