// SPDX-FileCopyrightText: 2020 Kurt
// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using DotLiquid;

namespace LiquidTestReports.Core.Drops
{
    public class LibraryDrop : Drop
    {
        public IDictionary<string, object> Parameters { get; set;  }

        public virtual string Text => Constants.LibraryText;

        public virtual string Link => Constants.LibraryLink;
    }
}
