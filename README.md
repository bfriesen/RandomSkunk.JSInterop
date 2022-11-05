# RandomSkunk.JSInterop

*Makes JavaScript interop a little easier with dynamic proxy objects.*

## Quick Start

Convert an `IJSRuntime` instance to `dynamic` with the `AsDynamic()` extension method, then call JavaScript methods and properties on that object directly. Values returned by functions are also dynamic and are used in the same manner.

```c#
using Microsoft.JSInterop;
using RandomSkunk.JSInterop;

async Task MountStripeCardElement(
    IJSRuntime jsRuntime,
    string stripePublicKey,
    string cardElementId = "#card-element")
{
    var js = jsRuntime.AsDynamic();
    var stripe = await js.Stripe(stripePublicKey);
    var elements = await stripe.elements();
    var cardElement = await elements.create("card");
    await cardElement.mount(cardElementId);
}
```

*(Example taken from [Stripe's documentation](https://stripe.com/docs/payments/accept-a-payment-charges?platform=web#web-create-payment-form) on how to setup a payment form)*

## Async vs. Sync

The dynamic proxy returned by the `IJSRuntime.AsDynamic()` extension method is always an *async* proxy. This means that all calls with it are made asynchronously and must be awaited. Furthermore, values returned by calls made to an async proxy are always async proxy objects, which make calls that must be awaited and themselves return async proxy objects.

To make synchronous calls that don't need to be awaited, you must meet two preconditions: 1) you must know that the JavaScript function is itself synchronous, and 2) the backing `IJSRuntime` or `IJSObjectReference` must also implement `IJSInProcessRuntime` or `IJSInProcessObjectReference` respectively. If both conditions are met, call the `AsSync()` method on the async proxy - it method returns a *sync* version of the async dynamic proxy. All calls with a sync proxy are made synchronously and must *not* be awaited. To get an async version of a sync proxy, call the `AsAsync()` method on the sync proxy.

```c#
async Task MountStripeCardElement(
    IJSRuntime jsRuntime,
    string stripePublicKey,
    string cardElementId = "#card-element")
{
    // Proxies are async by default:
    var js = jsRuntime.AsDynamic();

    // Calls to async proxies are awaited and return async proxies:
    var stripe = await js.Stripe(stripePublicKey);

    // Getting a sync version of an async proxy and making non-awaited call to it:
    var elements = stripe.AsSync().elements();

    // Calls to sync proxies are not awaited and return sync proxies:
    var cardElement = elements.create("card");

    // Getting an async version of a sync proxy and awaiting a call to it:
    await cardElement.AsAsync().mount(cardElementId);
}
```

*(This is the same example as the Quick Start, but with both sync and async calls.)*
