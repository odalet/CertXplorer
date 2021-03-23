//
// This code was written by Keith Brown, and may be freely used.
// Want to learn more about .NET? Visit pluralsight.com today!
//
using System;

namespace Pluralsight.Crypto
{
    public abstract class CryptKey : DisposeableObject
    {
        private readonly CryptContext context;

        protected CryptKey(CryptContext ctx, IntPtr handle)
        {
            context = ctx;
            Handle = handle;
        }

        public abstract KeyType Type { get; }
        internal IntPtr Handle { get; }

        protected override void CleanUp(bool viaDispose)
        {
            // keys are invalid once CryptContext is closed,
            // so the only time I try to close an individual key is if a user
            // explicitly disposes of the key.
            if (viaDispose)
                context.DestroyKey(this);
        }
    }

    public sealed class KeyExchangeKey : CryptKey
    {
        internal KeyExchangeKey(CryptContext ctx, IntPtr handle) : base(ctx, handle) { }

        public override KeyType Type => KeyType.Exchange;
    }

    public sealed class SignatureKey : CryptKey
    {
        internal SignatureKey(CryptContext ctx, IntPtr handle) : base(ctx, handle) { }

        public override KeyType Type => KeyType.Signature;
    }
}
