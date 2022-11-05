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
