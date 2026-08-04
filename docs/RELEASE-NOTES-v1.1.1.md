# Cirreum.Messaging 1.1.1 — a same-day correction: `ReceiverTuning` is creation-time only

## Why this release exists

1.1.0 shipped `ReceiverTuning` with two knobs. Implementing the Azure Service Bus mapping the
same day surfaced that one of them could never work: the receiver surface is pull-based, and
auto-lock-renewal is a push-processor feature — there is no creation-time renewal option on the
broker to map `MaxAutoLockRenewalDuration` onto. Left in place, the knob would have been
bindable, documented configuration that reaches nothing — the exact defect the receiver-tuning
chain exists to eliminate. It is removed before anyone adopts it.

## What changed

`ReceiverTuning` now carries creation-time broker options only: **`PrefetchCount`**.

Message-lock renewal is a *processing-time* behavior over the existing
`IMessagingReceivedMessage.RenewLockAsync`, owned by whoever runs the handler. The distributed
channel's receive loop (`Cirreum.Runtime.Messaging`) honors
`ReceiverOptions.MaxAutoLockRenewalDuration` there — so the configured value ends up genuinely
honored, at the layer that owns the handler lifetime, for every broker.

## Compatibility

Breaking only for code written against 1.1.0's `MaxAutoLockRenewalDuration` init-property in
the hours it existed — released same-day, pre-adoption, via the sanctioned
`-AllowBreakingPatch` path. Everything else in 1.1.0 is unchanged.

## See also

- `docs/RELEASE-NOTES-v1.1.0.md` — the receiver-tuning chain this corrects
- `docs/CHANGELOG.md` — the enumerated change
