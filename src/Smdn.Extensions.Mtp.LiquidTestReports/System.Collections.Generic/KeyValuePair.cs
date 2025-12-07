// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if !SYSTEM_COLLECTIONS_GENERIC_KEYVALUEPAIR_CREATE
namespace System.Collections.Generic;

public static class KeyValuePair {
  public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value)
    => new(key, value);
}
#endif
