namespace RandomSkunk.JSInterop.Tests
{
    public class SyncProxyJSObjectReference_class
    {
        public class Constructor
        {
            [Fact]
            public void When_IJSInProcessObjectReference_argument_is_not_null_Sets_JSObject()
            {
                // Arrange
                IJSInProcessObjectReference jsObject = new Mock<IJSInProcessObjectReference>().Object;

                // Act
                var proxyObject = new SyncProxyJSObjectReference(jsObject);

                // Assert
                proxyObject.JSObject.Should().BeSameAs(jsObject);
            }

            [Fact]
            public void When_IJSInProcessObjectReference_argument_is_null_Throws_ArgumentNullException()
            {
                // Arrange
                IJSInProcessObjectReference jsObject = null!;

                // Act
                var act = () => new SyncProxyJSObjectReference(jsObject);

                // Assert
                act.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        public class AsAsync_method
        {
            [Fact]
            public void When_jsObject_is_IJSInProcessObjectReference_Returns_equivalent_async_version()
            {
                // Arrange
                IJSInProcessObjectReference jsObject = new Mock<IJSInProcessObjectReference>().Object;

                SyncProxyJSObjectReference? syncProxyObject = new(jsObject);

                // Act
                AsyncProxyJSObjectReference? asyncProxyObject = syncProxyObject.AsAsync();

                // Assert
                asyncProxyObject.JSObject.Should().BeSameAs(jsObject);
            }
        }

        public class As_dynamic
        {
            public class JS_property_access
            {
                [Fact]
                public void When_property_returns_value_Returns_sync_proxy_object_of_it()
                {
                    // Arrange
                    IJSInProcessObjectReference returnedJSObjectReference = new Mock<IJSInProcessObjectReference>().Object;

                    Mock<IJSInProcessObjectReference> mockJSObject = new();
                    mockJSObject.Setup(m => m.Invoke<IJSInProcessObjectReference>("TestProperty", It.IsAny<object?[]>()))
                        .Returns(returnedJSObjectReference);

                    dynamic proxyObject = new SyncProxyJSObjectReference(mockJSObject.Object);

                    // Act
                    object actual = proxyObject.TestProperty;

                    // Assert
                    actual.Should().BeOfType<SyncProxyJSObjectReference>()
                        .Which.JSObject.Should().BeSameAs(returnedJSObjectReference);

                    mockJSObject.Verify(
                        m => m.Invoke<IJSInProcessObjectReference>(
                            "TestProperty",
                            It.Is<object?[]>(args => args == null || args.Length == 0)),
                        Times.Once());
                }

                /* TODO: Add test for when the js property returns null (and one for undefined). */

                [Fact]
                public void When_property_throws_Exception_is_not_caught()
                {
                    // Arrange
                    var exception = new Exception("Uh, oh.");

                    Mock<IJSInProcessObjectReference> mockJSObject = new();
                    mockJSObject.Setup(m => m.Invoke<IJSInProcessObjectReference>("TestProperty", It.IsAny<object?[]>()))
                        .Throws(exception);

                    dynamic proxyObject = new SyncProxyJSObjectReference(mockJSObject.Object);

                    // Act
                    var act = () => proxyObject.TestProperty;

                    // Assert
                    act.Should()
                        .ThrowExactly<Exception>()
                        .WithMessage("Uh, oh.");

                    mockJSObject.Verify(
                        m => m.Invoke<IJSInProcessObjectReference>(
                            "TestProperty",
                            It.Is<object?[]>(args => args == null || args.Length == 0)),
                        Times.Once());
                }
            }

            public class JS_function_invocation
            {
                [Fact]
                public void Given_non_generic_function_When_function_returns_value_Returns_sync_proxy_object_of_it()
                {
                    // Arrange
                    IJSInProcessObjectReference returnedJSObjectReference = new Mock<IJSInProcessObjectReference>().Object;

                    Mock<IJSInProcessObjectReference> mockJSObject = new();
                    mockJSObject.Setup(m => m.Invoke<IJSInProcessObjectReference>("TestFunction", It.IsAny<object?[]>()))
                        .Returns(returnedJSObjectReference);

                    dynamic proxyObject = new SyncProxyJSObjectReference(mockJSObject.Object);

                    // Act
                    object actual = proxyObject.TestFunction("abc", 123);

                    // Assert
                    actual.Should().BeOfType<SyncProxyJSObjectReference>()
                        .Which.JSObject.Should().BeSameAs(returnedJSObjectReference);

                    mockJSObject.Verify(
                        m => m.Invoke<IJSInProcessObjectReference>(
                            "TestFunction",
                            It.Is<object?[]>(args =>
                                args != null
                                && args.Length == 2
                                && "abc".Equals(args[0])
                                && 123.Equals(args[1]))),
                        Times.Once());
                }

                /* TODO: Add test for when the js property returns null (and one for undefined). */

                [Fact]
                public void Given_non_generic_function_When_function_throws_Exception_is_not_caught()
                {
                    // Arrange
                    var exception = new Exception("Uh, oh.");

                    Mock<IJSInProcessObjectReference> mockJSObject = new();
                    mockJSObject.Setup(m => m.Invoke<IJSInProcessObjectReference>("TestFunction", It.IsAny<object?[]>()))
                        .Throws(exception);

                    dynamic proxyObject = new SyncProxyJSObjectReference(mockJSObject.Object);

                    // Act
                    var act = () => proxyObject.TestFunction("abc", 123);

                    // Assert
                    act.Should()
                        .ThrowExactly<Exception>()
                        .WithMessage("Uh, oh.");

                    mockJSObject.Verify(
                        m => m.Invoke<IJSInProcessObjectReference>(
                            "TestFunction",
                            It.Is<object?[]>(args =>
                                args != null
                                && args.Length == 2
                                && "abc".Equals(args[0])
                                && 123.Equals(args[1]))),
                        Times.Once());
                }

                [Fact]
                public void Given_generic_function_When_function_returns_value_Returns_it()
                {
                    // Arrange
                    Mock<IJSInProcessObjectReference> mockJSObject = new();
                    mockJSObject.Setup(m => m.Invoke<bool>("TestFunction", It.IsAny<object?[]>()))
                        .Returns(true);

                    dynamic proxyObject = new SyncProxyJSObjectReference(mockJSObject.Object);

                    // Act
                    object actual = proxyObject.TestFunction<bool>("abc", 123);

                    // Assert
                    actual.Should().BeOfType<bool>()
                        .Which.Should().BeTrue();

                    mockJSObject.Verify(
                        m => m.Invoke<bool>(
                            "TestFunction",
                            It.Is<object?[]>(args =>
                                args != null
                                && args.Length == 2
                                && "abc".Equals(args[0])
                                && 123.Equals(args[1]))),
                        Times.Once());
                }

                /* TODO: Add test for when the js property returns null (and one for undefined). */

                [Fact]
                public void Given_generic_function_When_function_throws_Exception_is_not_caught()
                {
                    // Arrange
                    var exception = new Exception("Uh, oh.");

                    Mock<IJSInProcessObjectReference> mockJSObject = new();
                    mockJSObject.Setup(m => m.Invoke<bool>("TestFunction", It.IsAny<object?[]>()))
                        .Throws(exception);

                    dynamic proxyObject = new SyncProxyJSObjectReference(mockJSObject.Object);

                    // Act
                    var act = () => proxyObject.TestFunction<bool>("abc", 123);

                    // Assert
                    act.Should()
                        .ThrowExactly<TargetInvocationException>()
                        .WithInnerExceptionExactly<Exception>()
                        .WithMessage("Uh, oh.");

                    mockJSObject.Verify(
                        m => m.Invoke<bool>(
                            "TestFunction",
                            It.Is<object?[]>(args =>
                                args != null
                                && args.Length == 2
                                && "abc".Equals(args[0])
                                && 123.Equals(args[1]))),
                        Times.Once());
                }
            }

            public class Conversion
            {
                [Fact]
                public void Can_implicitly_convert_to_IJSInProcessObjectReference()
                {
                    // Arrange
                    IJSInProcessObjectReference jsObject = new Mock<IJSInProcessObjectReference>().Object;

                    dynamic proxyObject = new SyncProxyJSObjectReference(jsObject);

                    // Act
                    IJSInProcessObjectReference actual = proxyObject;

                    // Assert
                    actual.Should().BeSameAs(jsObject);
                }

                [Fact]
                public void Can_explicitly_convert_to_IJSInProcessObjectReference()
                {
                    // Arrange
                    IJSInProcessObjectReference jsObject = new Mock<IJSInProcessObjectReference>().Object;

                    dynamic proxyObject = new SyncProxyJSObjectReference(jsObject);

                    // Act
                    var actual = (IJSInProcessObjectReference)proxyObject;

                    // Assert
                    actual.Should().BeSameAs(jsObject);
                }
            }
        }
    }
}
