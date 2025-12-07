// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using NUnit.Framework;

[TestFixture]
public class Tests {
  [Test]
  public void TestCasePass()
  {
    var n = 0;

    Assert.That(n, Is.Zero);
  }

  [Test]
  public void TestCaseFail()
  {
    var n = 1;

    Assert.That(n, Is.Zero);
  }

  [Test]
  public void TestCaseSkipped()
  {
    Assert.Ignore();
  }
}
