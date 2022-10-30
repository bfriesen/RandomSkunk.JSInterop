namespace RandomSkunk.JSInterop.Tests
{
    public class AsyncProxyJSObjectReference_class
    {
        public class Constructor
        {
            [Fact]
            public void When_IJSObjectReference_argument_is_not_null_Sets_JSObject()
            {
                // Arrange
                IJSObjectReference jsObject = new Mock<IJSObjectReference>().Object;

                // Act
                var proxyJSObject = new AsyncProxyJSObjectReference(jsObject);

                // Assert
                proxyJSObject.JSObject.Should().BeSameAs(jsObject);
            }

            [Fact]
            public void When_IJSObjectReference_argument_is_null_Throws_ArgumentNullException()
            {
                // Arrange
                IJSObjectReference jsObject = null!;

                // Act
                var act = () => new AsyncProxyJSObjectReference(jsObject);

                // Assert
                act.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        public class Sync_method
        {
            [Fact]
            public void When_jsObject_is_IJSInProcessObjectReference_Returns_equivalent_sync_version()
            {
                // Arrange
                IJSObjectReference jsObject = new Mock<IJSInProcessObjectReference>().Object;

                AsyncProxyJSObjectReference? asyncProxyObject = new(jsObject);

                // Act
                SyncProxyJSObjectReference? syncProxyObject = asyncProxyObject.Sync();

                // Assert
                syncProxyObject.JSObject.Should().BeSameAs(jsObject);
            }

            [Fact]
            public void When_jsObject_is_not_IJSInProcessObjectReference_Throws_InvalidOperationException()
            {
                // Arrange
                IJSObjectReference jsObject = new Mock<IJSObjectReference>().Object;

                AsyncProxyJSObjectReference? asyncProxyObject = new(jsObject);

                // Act / Assert
                asyncProxyObject.Invoking(m => m.Sync()).Should()
                    .ThrowExactly<InvalidOperationException>();
            }
        }

        public class As_dynamic
        {
            public class JS_property_access
            {
                [Fact]
                public async Task When_property_returns_value_Returns_async_proxy_object_of_it()
                {
                    // Arrange
                    IJSObjectReference returnedJSObjectReference = new Mock<IJSObjectReference>().Object;

                    Mock<IJSObjectReference> mockJSObject = new();
                    mockJSObject.Setup(m => m.InvokeAsync<IJSObjectReference>("TestProperty", It.IsAny<object?[]>()))
                        .ReturnsAsync(returnedJSObjectReference);

                    dynamic proxyJSObject = new AsyncProxyJSObjectReference(mockJSObject.Object);

                    // Act
                    object actual = await proxyJSObject.TestProperty;

                    // Assert
                    actual.Should().BeOfType<AsyncProxyJSObjectReference>()
                        .Which.JSObject.Should().BeSameAs(returnedJSObjectReference);

                    mockJSObject.Verify(
                        m => m.InvokeAsync<IJSObjectReference>(
                            "TestProperty",
                            It.Is<object?[]>(args => args == null || args.Length == 0)),
                        Times.Once());
                }

                /* TODO: Add test for when the js property returns null (and one for undefined). */

                [Fact]
                public async Task When_property_throws_Exception_is_not_caught()
                {
                    // Arrange
                    var exception = new Exception("Uh, oh.");

                    Mock<IJSObjectReference> mockJSObject = new();
                    mockJSObject.Setup(m => m.InvokeAsync<IJSObjectReference>("TestProperty", It.IsAny<object?[]>()))
                        .ThrowsAsync(exception);

                    dynamic proxyJSObject = new AsyncProxyJSObjectReference(mockJSObject.Object);

                    // Act
                    var act = async () => await proxyJSObject.TestProperty;

                    // Assert
                    await act.Should()
                        .ThrowExactlyAsync<AggregateException>()
                        .WithInnerExceptionExactly<AggregateException, Exception>()
                        .WithMessage("Uh, oh.");

                    mockJSObject.Verify(
                        m => m.InvokeAsync<IJSObjectReference>(
                            "TestProperty",
                            It.Is<object?[]>(args => args == null || args.Length == 0)),
                        Times.Once());
                }
            }

            public class JS_function_invocation
            {
                [Fact]
                public async Task Given_non_generic_function_When_function_returns_value_Returns_async_proxy_object_of_it()
                {
                    // Arrange
                    IJSObjectReference returnedJSObjectReference = new Mock<IJSObjectReference>().Object;

                    Mock<IJSObjectReference> mockJSObject = new();
                    mockJSObject.Setup(m => m.InvokeAsync<IJSObjectReference>("TestFunction", It.IsAny<object?[]>()))
                        .ReturnsAsync(returnedJSObjectReference);

                    dynamic proxyJSObject = new AsyncProxyJSObjectReference(mockJSObject.Object);

                    // Act
                    object actual = await proxyJSObject.TestFunction("abc", 123);

                    // Assert
                    actual.Should().BeOfType<AsyncProxyJSObjectReference>()
                        .Which.JSObject.Should().BeSameAs(returnedJSObjectReference);

                    mockJSObject.Verify(
                        m => m.InvokeAsync<IJSObjectReference>(
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
                public async Task Given_non_generic_function_When_function_throws_Exception_is_not_caught()
                {
                    // Arrange
                    var exception = new Exception("Uh, oh.");

                    Mock<IJSObjectReference> mockJSObject = new();
                    mockJSObject.Setup(m => m.InvokeAsync<IJSObjectReference>("TestFunction", It.IsAny<object?[]>()))
                        .ThrowsAsync(exception);

                    dynamic proxyJSObject = new AsyncProxyJSObjectReference(mockJSObject.Object);

                    // Act
                    var act = async () => await proxyJSObject.TestFunction("abc", 123);

                    // Assert
                    await act.Should()
                        .ThrowExactlyAsync<AggregateException>()
                        .WithInnerExceptionExactly<AggregateException, Exception>()
                        .WithMessage("Uh, oh.");

                    mockJSObject.Verify(
                        m => m.InvokeAsync<IJSObjectReference>(
                            "TestFunction",
                            It.Is<object?[]>(args =>
                                args != null
                                && args.Length == 2
                                && "abc".Equals(args[0])
                                && 123.Equals(args[1]))),
                        Times.Once());
                }

                [Fact]
                public async Task Given_generic_function_When_function_returns_value_Returns_it()
                {
                    // Arrange
                    Mock<IJSObjectReference> mockJSObject = new();
                    mockJSObject.Setup(m => m.InvokeAsync<bool>("TestFunction", It.IsAny<object?[]>()))
                        .ReturnsAsync(true);

                    dynamic proxyJSObject = new AsyncProxyJSObjectReference(mockJSObject.Object);

                    // Act
                    object actual = await proxyJSObject.TestFunction<bool>("abc", 123);

                    // Assert
                    actual.Should().BeOfType<bool>()
                        .Which.Should().BeTrue();

                    mockJSObject.Verify(
                        m => m.InvokeAsync<bool>(
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
                public async Task Given_generic_function_When_function_throws_Exception_is_not_caught()
                {
                    // Arrange
                    var exception = new Exception("Uh, oh.");

                    Mock<IJSObjectReference> mockJSObject = new();
                    mockJSObject.Setup(m => m.InvokeAsync<bool>("TestFunction", It.IsAny<object?[]>()))
                        .ThrowsAsync(exception);

                    dynamic proxyJSObject = new AsyncProxyJSObjectReference(mockJSObject.Object);

                    // Act
                    var act = async () => await proxyJSObject.TestFunction<bool>("abc", 123);

                    // Assert
                    await act.Should()
                        .ThrowExactlyAsync<Exception>()
                        .WithMessage("Uh, oh.");

                    mockJSObject.Verify(
                        m => m.InvokeAsync<bool>(
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
                public void Can_implicitly_convert_to_IJSObjectReference()
                {
                    // Arrange
                    IJSObjectReference jsObject = new Mock<IJSObjectReference>().Object;

                    dynamic proxyJSObject = new AsyncProxyJSObjectReference(jsObject);

                    // Act
                    IJSObjectReference actual = proxyJSObject;

                    // Assert
                    actual.Should().BeSameAs(jsObject);
                }

                [Fact]
                public void Can_explicitly_convert_to_IJSObjectReference()
                {
                    // Arrange
                    IJSObjectReference jsObject = new Mock<IJSObjectReference>().Object;

                    dynamic proxyJSObject = new AsyncProxyJSObjectReference(jsObject);

                    // Act
                    var actual = (IJSObjectReference)proxyJSObject;

                    // Assert
                    actual.Should().BeSameAs(jsObject);
                }
            }
        }
    }
}
