namespace RandomSkunk.JSInterop.Tests
{
    public class SyncProxyJSRuntime_class
    {
        public class Constructor
        {
            [Fact]
            public void When_IJSInProcessRuntime_argument_is_not_null_Sets_JSRuntime()
            {
                // Arrange
                IJSInProcessRuntime jsRuntime = new Mock<IJSInProcessRuntime>().Object;

                // Act
                var proxyRuntime = new SyncProxyJSRuntime(jsRuntime);

                // Assert
                proxyRuntime.JSRuntime.Should().BeSameAs(jsRuntime);
            }

            [Fact]
            public void When_IJSInProcessRuntime_argument_is_null_Throws_ArgumentNullException()
            {
                // Arrange
                IJSInProcessRuntime jsRuntime = null!;

                // Act
                var act = () => new SyncProxyJSRuntime(jsRuntime);

                // Assert
                act.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        public class AsAsync_method
        {
            [Fact]
            public void When_jsRuntime_is_IJSInProcessRuntime_Returns_equivalent_async_version()
            {
                // Arrange
                IJSInProcessRuntime jsRuntime = new Mock<IJSInProcessRuntime>().Object;

                SyncProxyJSRuntime? syncProxyRuntime = new(jsRuntime);

                // Act
                AsyncProxyJSRuntime? asyncProxyRuntime = syncProxyRuntime.AsAsync();

                // Assert
                asyncProxyRuntime.JSRuntime.Should().BeSameAs(jsRuntime);
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

                    Mock<IJSInProcessRuntime> mockJSRuntime = new();
                    mockJSRuntime.Setup(m => m.Invoke<IJSInProcessObjectReference>("TestProperty", It.IsAny<object?[]>()))
                        .Returns(returnedJSObjectReference);

                    dynamic proxyRuntime = new SyncProxyJSRuntime(mockJSRuntime.Object);

                    // Act
                    object actual = proxyRuntime.TestProperty;

                    // Assert
                    actual.Should().BeOfType<SyncProxyJSObjectReference>()
                        .Which.JSObject.Should().BeSameAs(returnedJSObjectReference);

                    mockJSRuntime.Verify(
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

                    Mock<IJSInProcessRuntime> mockJSRuntime = new();
                    mockJSRuntime.Setup(m => m.Invoke<IJSInProcessObjectReference>("TestProperty", It.IsAny<object?[]>()))
                        .Throws(exception);

                    dynamic proxyRuntime = new SyncProxyJSRuntime(mockJSRuntime.Object);

                    // Act
                    var act = () => proxyRuntime.TestProperty;

                    // Assert
                    act.Should()
                        .ThrowExactly<Exception>()
                        .WithMessage("Uh, oh.");

                    mockJSRuntime.Verify(
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

                    Mock<IJSInProcessRuntime> mockJSRuntime = new();
                    mockJSRuntime.Setup(m => m.Invoke<IJSInProcessObjectReference>("TestFunction", It.IsAny<object?[]>()))
                        .Returns(returnedJSObjectReference);

                    dynamic proxyRuntime = new SyncProxyJSRuntime(mockJSRuntime.Object);

                    // Act
                    object actual = proxyRuntime.TestFunction("abc", 123);

                    // Assert
                    actual.Should().BeOfType<SyncProxyJSObjectReference>()
                        .Which.JSObject.Should().BeSameAs(returnedJSObjectReference);

                    mockJSRuntime.Verify(
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

                    Mock<IJSInProcessRuntime> mockJSRuntime = new();
                    mockJSRuntime.Setup(m => m.Invoke<IJSInProcessObjectReference>("TestFunction", It.IsAny<object?[]>()))
                        .Throws(exception);

                    dynamic proxyRuntime = new SyncProxyJSRuntime(mockJSRuntime.Object);

                    // Act
                    var act = () => proxyRuntime.TestFunction("abc", 123);

                    // Assert
                    act.Should()
                        .ThrowExactly<Exception>()
                        .WithMessage("Uh, oh.");

                    mockJSRuntime.Verify(
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
                    Mock<IJSInProcessRuntime> mockJSRuntime = new();
                    mockJSRuntime.Setup(m => m.Invoke<bool>("TestFunction", It.IsAny<object?[]>()))
                        .Returns(true);

                    dynamic proxyRuntime = new SyncProxyJSRuntime(mockJSRuntime.Object);

                    // Act
                    object actual = proxyRuntime.TestFunction<bool>("abc", 123);

                    // Assert
                    actual.Should().BeOfType<bool>()
                        .Which.Should().BeTrue();

                    mockJSRuntime.Verify(
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

                    Mock<IJSInProcessRuntime> mockJSRuntime = new();
                    mockJSRuntime.Setup(m => m.Invoke<bool>("TestFunction", It.IsAny<object?[]>()))
                        .Throws(exception);

                    dynamic proxyRuntime = new SyncProxyJSRuntime(mockJSRuntime.Object);

                    // Act
                    var act = () => proxyRuntime.TestFunction<bool>("abc", 123);

                    // Assert
                    act.Should()
                        .ThrowExactly<TargetInvocationException>()
                        .WithInnerExceptionExactly<Exception>()
                        .WithMessage("Uh, oh.");

                    mockJSRuntime.Verify(
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
                public void Can_implicitly_convert_to_IJSInProcessRuntime()
                {
                    // Arrange
                    IJSInProcessRuntime jsRuntime = new Mock<IJSInProcessRuntime>().Object;

                    dynamic proxyRuntime = new SyncProxyJSRuntime(jsRuntime);

                    // Act
                    IJSInProcessRuntime actual = proxyRuntime;

                    // Assert
                    actual.Should().BeSameAs(jsRuntime);
                }

                [Fact]
                public void Can_explicitly_convert_to_IJSInProcessRuntime()
                {
                    // Arrange
                    IJSInProcessRuntime jsRuntime = new Mock<IJSInProcessRuntime>().Object;

                    dynamic proxyRuntime = new SyncProxyJSRuntime(jsRuntime);

                    // Act
                    var actual = (IJSInProcessRuntime)proxyRuntime;

                    // Assert
                    actual.Should().BeSameAs(jsRuntime);
                }
            }
        }
    }
}
