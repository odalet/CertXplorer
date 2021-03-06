﻿using System;

namespace Delta.CertXplorer.DocumentModel
{
    public interface IDocumentHandlerRegistryService
    {
        void Register(Func<IDocumentHandler> handlerBuilder, int priority = 0);
        IDocumentHandler[] Find(IDocumentSource source);
    }
}
