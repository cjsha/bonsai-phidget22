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
    public class RotaryEncoderEventDriven : Source<long>
    {
        [Description("")]
        [Range(1, 600)]
        public int PositionChangeTrigger { get; set; } = 1;

        [Description("")]
        public float SampleFrequencyHz { get; set; } = 100;

        [Description("")]
        [Range(0, 5)]
        public int HubPort { get; set; } = 0;

        public override IObservable<long> Generate()
        {
            var encoder = new Encoder()
            {
                HubPort = HubPort,
                DeviceSerialNumber = 767469
            };

            encoder.Open(Phidget.DefaultTimeout);
            encoder.PositionChangeTrigger = PositionChangeTrigger;
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