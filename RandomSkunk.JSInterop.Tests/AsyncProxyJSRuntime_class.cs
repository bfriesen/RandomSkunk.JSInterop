namespace RandomSkunk.JSInterop.Tests
{
    public class AsyncProxyJSRuntime_class
    {
        public class Constructor
        {
            [Fact]
            public void When_IJSRuntime_argument_is_not_null_Sets_JSRuntime()
            {
                // Arrange
                IJSRuntime jsRuntime = new Mock<IJSRuntime>().Object;

                // Act
                var proxyRuntime = new AsyncProxyJSRuntime(jsRuntime);

                // Assert
                proxyRuntime.JSRuntime.Should().BeSameAs(jsRuntime);
            }

            [Fact]
            public void When_IJSRuntime_argument_is_null_Throws_ArgumentNullException()
            {
                // Arrange
                IJSRuntime jsRuntime = null!;

                // Act
                var act = () => new AsyncProxyJSRuntime(jsRuntime);

                // Assert
                act.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        public class AsSync_method
        {
            [Fact]
            public void When_jsRuntime_is_IJSInProcessRuntime_Returns_equivalent_sync_version()
            {
                // Arrange
                IJSInProcessRuntime jsRuntime = new Mock<IJSInProcessRuntime>().Object;

                AsyncProxyJSRuntime? asyncProxyRuntime = new(jsRuntime);

                // Act
                SyncProxyJSRuntime? syncProxyRuntime = asyncProxyRuntime.AsSync();

                // Assert
                syncProxyRuntime.JSRuntime.Should().BeSameAs(jsRuntime);
            }

            [Fact]
            public void When_jsRuntime_is_not_IJSInProcessRuntime_Throws_InvalidOperationException()
            {
                // Arrange
                IJSRuntime jsRuntime = new Mock<IJSRuntime>().Object;

                AsyncProxyJSRuntime? asyncProxyRuntime = new(jsRuntime);

                // Act / Assert
                asyncProxyRuntime.Invoking(m => m.AsSync()).Should()
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

                    Mock<IJSRuntime> mockJSRuntime = new();
                    mockJSRuntime.Setup(m => m.InvokeAsync<IJSObjectReference>("TestProperty", It.IsAny<object?[]>()))
                        .ReturnsAsync(returnedJSObjectReference);

                    dynamic proxyRuntime = new AsyncProxyJSRuntime(mockJSRuntime.Object);

                    // Act
                    object actual = await proxyRuntime.TestProperty;

                    // Assert
                    actual.Should().BeOfType<AsyncProxyJSObjectReference>()
                        .Which.JSObject.Should().BeSameAs(returnedJSObjectReference);

                    mockJSRuntime.Verify(
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

                    Mock<IJSRuntime> mockJSRuntime = new();
                    mockJSRuntime.Setup(m => m.InvokeAsync<IJSObjectReference>("TestProperty", It.IsAny<object?[]>()))
                        .ThrowsAsync(exception);

                    dynamic proxyRuntime = new AsyncProxyJSRuntime(mockJSRuntime.Object);

                    // Act
                    var act = async () => await proxyRuntime.TestProperty;

                    // Assert
                    await act.Should()
                        .ThrowExactlyAsync<AggregateException>()
                        .WithInnerExceptionExactly<AggregateException, Exception>()
                        .WithMessage("Uh, oh.");

                    mockJSRuntime.Verify(
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

                    Mock<IJSRuntime> mockJSRuntime = new();
                    mockJSRuntime.Setup(m => m.InvokeAsync<IJSObjectReference>("TestFunction", It.IsAny<object?[]>()))
                        .ReturnsAsync(returnedJSObjectReference);

                    dynamic proxyRuntime = new AsyncProxyJSRuntime(mockJSRuntime.Object);

                    // Act
                    object actual = await proxyRuntime.TestFunction("abc", 123);

                    // Assert
                    actual.Should().BeOfType<AsyncProxyJSObjectReference>()
                        .Which.JSObject.Should().BeSameAs(returnedJSObjectReference);

                    mockJSRuntime.Verify(
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

                    Mock<IJSRuntime> mockJSRuntime = new();
                    mockJSRuntime.Setup(m => m.InvokeAsync<IJSObjectReference>("TestFunction", It.IsAny<object?[]>()))
                        .ThrowsAsync(exception);

                    dynamic proxyRuntime = new AsyncProxyJSRuntime(mockJSRuntime.Object);

                    // Act
                    var act = async () => await proxyRuntime.TestFunction("abc", 123);

                    // Assert
                    await act.Should()
                        .ThrowExactlyAsync<AggregateException>()
                        .WithInnerExceptionExactly<AggregateException, Exception>()
                        .WithMessage("Uh, oh.");

                    mockJSRuntime.Verify(
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
                    Mock<IJSRuntime> mockJSRuntime = new();
                    mockJSRuntime.Setup(m => m.InvokeAsync<bool>("TestFunction", It.IsAny<object?[]>()))
                        .ReturnsAsync(true);

                    dynamic proxyRuntime = new AsyncProxyJSRuntime(mockJSRuntime.Object);

                    // Act
                    object actual = await proxyRuntime.TestFunction<bool>("abc", 123);

                    // Assert
                    actual.Should().BeOfType<bool>()
                        .Which.Should().BeTrue();

                    mockJSRuntime.Verify(
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

                    Mock<IJSRuntime> mockJSRuntime = new();
                    mockJSRuntime.Setup(m => m.InvokeAsync<bool>("TestFunction", It.IsAny<object?[]>()))
                        .ThrowsAsync(exception);

                    dynamic proxyRuntime = new AsyncProxyJSRuntime(mockJSRuntime.Object);

                    // Act
                    var act = async () => await proxyRuntime.TestFunction<bool>("abc", 123);

                    // Assert
                    await act.Should()
                        .ThrowExactlyAsync<Exception>()
                        .WithMessage("Uh, oh.");

                    mockJSRuntime.Verify(
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
                public void Can_implicitly_convert_to_IJSRuntime()
                {
                    // Arrange
                    IJSRuntime jsRuntime = new Mock<IJSRuntime>().Object;

                    dynamic proxyRuntime = new AsyncProxyJSRuntime(jsRuntime);

                    // Act
                    IJSRuntime actual = proxyRuntime;

                    // Assert
                    actual.Should().BeSameAs(jsRuntime);
                }

                [Fact]
                public void Can_explicitly_convert_to_IJSRuntime()
                {
                    // Arrange
                    IJSRuntime jsRuntime = new Mock<IJSRuntime>().Object;

                    dynamic proxyRuntime = new AsyncProxyJSRuntime(jsRuntime);

                    // Act
                    var actual = (IJSRuntime)proxyRuntime;

                    // Assert
                    actual.Should().BeSameAs(jsRuntime);
                }
            }
        }
    }
}
