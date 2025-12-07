// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize] // suppress warning MSTEST0001

[TestClass]
public class Tests {
  [TestMethod]
  public void TestCasePass()
  {
    var n = 0;

    Assert.AreEqual(0, n);
  }

  [TestMethod]
  public void TestCaseFail()
  {
    var n = 1;

    Assert.AreEqual(0, n);
  }

  [TestMethod]
  public void TestCaseSkipped()
  {
    Assert.Inconclusive();
  }
}
