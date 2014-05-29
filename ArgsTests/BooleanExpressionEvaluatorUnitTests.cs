﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerArgs;
using System.Collections.Generic;

namespace ArgsTests
{
    [TestClass]
    public class BooleanExpressionEvaluatorUnitTests
    {
        [TestMethod]
        public void TestBooleanExpressionsSimpleOr()
        {
            Dictionary<string, bool> variableValues = new Dictionary<string, bool>
            {
                {"a", false},
                {"bb", false},
            };
            var parseResult = BooleanExpressionParser.Parse("a|bb");
            Assert.IsFalse(parseResult.Evaluate(variableValues));

            variableValues["a"] = true;
            Assert.IsTrue(parseResult.Evaluate(variableValues));
        }

        [TestMethod]
        public void TestBooleanExpressionsSimpleNot()
        {
            Dictionary<string, bool> variableValues = new Dictionary<string, bool>
            {
                {"a", false},
                {"bb", false},
            };
            var parseResult = BooleanExpressionParser.Parse("!a|bb");
            Assert.IsTrue(parseResult.Evaluate(variableValues));

            variableValues["a"] = true;
            Assert.IsFalse(parseResult.Evaluate(variableValues));
        }

        [TestMethod]
        public void TestBooleanExpressionsSimpleAnd()
        {
            Dictionary<string, bool> variableValues = new Dictionary<string, bool>
            {
                {"a", false},
                {"bb", false},
            };
            var parseResult = BooleanExpressionParser.Parse("a&bb");
            Assert.IsFalse(parseResult.Evaluate(variableValues));

            variableValues["a"] = true;
            Assert.IsFalse(parseResult.Evaluate(variableValues));

            variableValues["bb"] = true;
            Assert.IsTrue(parseResult.Evaluate(variableValues));
        }

        [TestMethod]
        public void TestBooleanExpressionsSimpleGroups()
        {
            Dictionary<string, bool> variableValues = new Dictionary<string, bool>
            {
                {"a", false},
                {"bb", false},
            };
            var parseResult = BooleanExpressionParser.Parse("(a)&(bb)");
            Assert.IsFalse(parseResult.Evaluate(variableValues));

            variableValues["a"] = true;
            Assert.IsFalse(parseResult.Evaluate(variableValues));

            variableValues["bb"] = true;
            Assert.IsTrue(parseResult.Evaluate(variableValues));
        }

        [TestMethod]
        public void TestBooleanExpressionsGroupsWithNot()
        {
            Dictionary<string, bool> variableValues = new Dictionary<string, bool>
            {
                {"a", false},
                {"bb", false},
            };
            var parseResult = BooleanExpressionParser.Parse("!(a)&!(bb)");
            Assert.IsTrue(parseResult.Evaluate(variableValues));

            variableValues["a"] = true;
            Assert.IsFalse(parseResult.Evaluate(variableValues));

            variableValues["bb"] = true;
            Assert.IsFalse(parseResult.Evaluate(variableValues));
        }

        [TestMethod]
        public void TestBooleanExpressionsComplexGroups()
        {
            Dictionary<string, bool> variableValues = new Dictionary<string, bool>
            {
                {"a", false},
                {"bb", false},
                {"c", false},
            };
            var parseResult = BooleanExpressionParser.Parse("( (a)&(bb) ) | c");
            Assert.IsFalse(parseResult.Evaluate(variableValues));

            variableValues["c"] = true;
            Assert.IsTrue(parseResult.Evaluate(variableValues));
            variableValues["c"] = false;


            variableValues["a"] = true;
            Assert.IsFalse(parseResult.Evaluate(variableValues));

            variableValues["bb"] = true;
            Assert.IsTrue(parseResult.Evaluate(variableValues));

            variableValues["c"] = true;
            Assert.IsTrue(parseResult.Evaluate(variableValues));
        }

        [TestMethod]
        public void TestBooleanExpressionsComplexGroupsWithNots()
        {
            Dictionary<string, bool> variableValues = new Dictionary<string, bool>
            {
                {"a", false},
                {"bb", false},
                {"c", false},
            };
            var parseResult = BooleanExpressionParser.Parse("!( ((a)&(bb)) | c )");
            Assert.IsTrue(parseResult.Evaluate(variableValues));

            variableValues["c"] = true;
            Assert.IsFalse(parseResult.Evaluate(variableValues));
            variableValues["c"] = false;


            variableValues["a"] = true;
            Assert.IsTrue(parseResult.Evaluate(variableValues));

            variableValues["bb"] = true;
            Assert.IsFalse(parseResult.Evaluate(variableValues));

            variableValues["c"] = true;
            Assert.IsFalse(parseResult.Evaluate(variableValues));
        }


        [TestMethod]
        public void TestBooleanExpressionsGroupLeftOpen()
        {
            try
            {
                BooleanExpressionParser.Parse("( (a)&(bb ) | c");
                Assert.Fail("An exception should have been thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("You are missing at least 1 closing parenthesis", ex.Message);
            }
        }

        [TestMethod]
        public void TestBooleanExpressionsGroupTooManyCloses()
        {
            try
            {
                BooleanExpressionParser.Parse(" (a)&(bb) ) | c");
                Assert.Fail("An exception should have been thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Unexpected token ')'", ex.Message);
            }
        }


        [TestMethod]
        public void TestBooleanExpressionsEmptyGroup()
        {
            try
            {
                BooleanExpressionParser.Parse("(a)|()");
                Assert.Fail("An exception should have been thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("You have an empty set of parenthesis", ex.Message);
            }
        }

        [TestMethod]
        public void TestBooleanExpressionsWrongOperator()
        {
            try
            {
                var parsed = BooleanExpressionParser.Parse("(a)||(b)");
                Assert.Fail("An exception should have been thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("You cannot have two consecutive operators '&&' or '||', use '&' or '|'", ex.Message);
            }

            try
            {
                var parsed = BooleanExpressionParser.Parse("(a && b)");
                Assert.Fail("An exception should have been thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("You cannot have two consecutive operators '&&' or '||', use '&' or '|'", ex.Message);
            }

            try
            {
                var parsed = BooleanExpressionParser.Parse("(a & & b)");
                Assert.Fail("An exception should have been thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("You cannot have two consecutive operators '&&' or '||', use '&' or '|'", ex.Message);
            }
        }

        [TestMethod]
        public void TestBooleanExpressionsConsecutiveNots()
        {
            try
            {
                BooleanExpressionParser.Parse("!!a");
                Assert.Fail("An exception should have been thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("You cannot have two consecutive '!' operators", ex.Message);
            }
        }

        [TestMethod]
        public void TestBooleanExpressionsTrailingNot()
        {
            try
            {
                BooleanExpressionParser.Parse("a!");
                Assert.Fail("An exception should have been thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Unexpected token '!' at end of expression", ex.Message);
            }
        }

        [TestMethod]
        public void TestBooleanExpressionsNotThenBooleanOperator()
        {
            try
            {
                BooleanExpressionParser.Parse("a!&b");
                Assert.Fail("An exception should have been thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("You cannot have an operator '&' or '|' after a '!'", ex.Message);
            }
        }
    }
}