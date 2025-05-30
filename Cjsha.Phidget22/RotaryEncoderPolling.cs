using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Phidget22;
using System.Reactive;

namespace Cjsha.Phidget22
{
    [Description("")]
    [Combinator(MethodName = nameof(Generate))]
    [WorkflowElementCategory(ElementCategory.Source)]
    public class RotaryEncoderPolling
    {
        [Description("")]
        [Range(0, 5)]
        public int HubPort { get; set; } = 0;

        public static IDisposable SubscribeSafe<TSource, TResult>(
            IObservable<TSource> source,
            IObserver<TResult> observer,
            Action<TSource> onNext)
        {
            var sourceObserver = Observer.Create<TSource>(
                value =>
                {
                    try { onNext(value); }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        }

        public unsafe IObservable<long> Generate<TSource>(IObservable<TSource> source)
        {
            var encoder = new Encoder()
            {
                HubPort = HubPort,
                DeviceSerialNumber = 767469
            };
            encoder.Open(Phidget.DefaultTimeout);

            return Observable.Create<long>(observer =>
            {
                return SubscribeSafe(source, observer, _ =>
                {
                    observer.OnNext(encoder.Position);
                });
            })
            .Finally(() => encoder.Close());
        }
    }
}