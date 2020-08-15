﻿// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MessagePack
{
    /// <summary>
    /// An interface optionally implemented by objects to be serialized or deserialized to receive callbacks from the serializer.
    /// </summary>
    public interface IMessagePackSerializationCallbackReceiver
    {
        /// <summary>
        /// Invoked before properties and fields are read for serialization.
        /// </summary>
        void OnBeforeSerialize();

        /// <summary>
        /// Invoked after properties and fields have been written by deserialization.
        /// </summary>
        void OnAfterDeserialize();
    }
}
