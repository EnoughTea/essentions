using System;
using System.ComponentModel;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class DelegateExtensionsTests
    {
        private static bool _called;

        private readonly EventHandler _emptyHandler = delegate { _called = true; };
        private readonly EventHandler<int> _handler = delegate { _called = true; };

        private readonly Action _actZero = delegate { _called = true; };
        private readonly Action<int> _actOne = delegate { _called = true; };
        private readonly Action<int, int> _actTwo = delegate { _called = true; };
        private readonly Action<int, int, int> _actThree = delegate { _called = true; };
        private readonly Action<int, int, int, int> _actFour = delegate { _called = true; };
        private readonly Action<int, int, int, int, int> _actFive = delegate { _called = true; };
        private readonly Action<int, int, int, int, int, int> _actSix = delegate { _called = true; };

        private readonly Func<int> _funcZero = delegate { _called = true; return 0; };
        private readonly Func<int, int> _funcOne = delegate { _called = true; return 0; };
        private readonly Func<int, int, int> _funcTwo = delegate { _called = true; return 0; };
        private readonly Func<int, int, int, int> _funcThree = delegate { _called = true; return 0; };
        private readonly Func<int, int, int, int, int> _funcFour = delegate { _called = true; return 0; };
        private readonly Func<int, int, int, int, int, int> _funcFive = delegate { _called = true; return 0; };
        private readonly Func<int, int, int, int, int, int, int> _funcSix = delegate { _called = true; return 0; };

        private readonly PropertyChangedEventHandler _propertyChanged = delegate { _called = true; };

        [SetUp]
        public void Init()
        {
            _called = false;
        }

        [Test]
        public void When_Call_is_called_on_empty_EventHandler_Then_delegate_is_invoked()
        {
            _emptyHandler.Call();
            Assert.That(_called, Is.True);

            _called = false;
            _emptyHandler.Call(new object(), EventArgs.Empty);
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_Call_is_called_on_non_empty_EventHandler_Then_delegate_is_invoked()
        {
            _handler.Call(0);
            Assert.That(_called, Is.True);

            _called = false;
            _handler.Call(new object(), 0);
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_Call_is_called_on_0_arg_action_Then_delegate_is_invoked()
        {
            _actZero.Call();
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_Call_is_called_on_1_arg_action_Then_delegate_is_invoked()
        {
            _actOne.Call(0);
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_Call_is_called_on_2_arg_action_Then_delegate_is_invoked()
        {
            _actTwo.Call(0, 0);
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_Call_is_called_on_3_arg_action_Then_delegate_is_invoked()
        {
            _actThree.Call(0, 0, 0);
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_Call_is_called_on_4_arg_action_Then_delegate_is_invoked()
        {
            _actFour.Call(0, 0, 0, 0);
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_Call_is_called_on_5_arg_action_Then_delegate_is_invoked()
        {
            _actFive.Call(0, 0, 0, 0, 0);
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_Call_is_called_on_6_arg_action_Then_delegate_is_invoked()
        {
            _actSix.Call(0, 0, 0, 0, 0, 0);
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_Call_is_called_on_0_arg_function_Then_delegate_is_invoked()
        {
            _funcZero.Call();
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_Call_is_called_on_1_arg_function_Then_delegate_is_invoked()
        {
            _funcOne.Call(0);
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_Call_is_called_on_2_arg_function_Then_delegate_is_invoked()
        {
            _funcTwo.Call(0, 0);
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_Call_is_called_on_3_arg_function_Then_delegate_is_invoked()
        {
            _funcThree.Call(0, 0, 0);
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_Call_is_called_on_4_arg_function_Then_delegate_is_invoked()
        {
            _funcFour.Call(0, 0, 0, 0);
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_Call_is_called_on_5_arg_function_Then_delegate_is_invoked()
        {
            _funcFive.Call(0, 0, 0, 0, 0);
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_Call_is_called_on_6_arg_function_Then_delegate_is_invoked()
        {
            _funcSix.Call(0, 0, 0, 0, 0, 0);
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_PropertyChangedEventHandler_is_called_Then_delegate_is_invoked()
        {
            _propertyChanged.Call(null, null);
            Assert.That(_called, Is.True);

            _called = false;
            _propertyChanged.Call(() => _funcSix);
            Assert.That(_called, Is.True);
        }

        [Test]
        public void When_PropertyChangedEventHandler_is_called_with_non_member_expression_Then_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(() => _propertyChanged.Call(() => 0));
        }
    }
}