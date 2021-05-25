using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payment.Tracker.BusinessLogic.Dto.Auth;
using Payment.Tracker.BusinessLogic.Dto.Payment;
using Payment.Tracker.Utils.EnvironmentVariables;

namespace Payment.Tracker.Utils.UnitTests.EnvironmentVariables
{
    [TestClass]
    public class EnvironmentVariablesHelperTests
    {
        [TestMethod]
        public void SetValueFromEnvVar_ShouldSetVariable_WhenExists()
        {
            // Arrange
            var tokenDto = new TokenDto();
            Environment.SetEnvironmentVariable("TEST", "test");
            
            // Act
            EnvironmentVariablesHelper.SetValueFromEnvVar<TokenDto, string>(tokenDto, dto => dto.Token, "TEST");
            
            // Assert
            tokenDto.Token.Should().Be("test");
        }

        [TestMethod]
        public void SetValueFromEnvVar_ShouldSetVariable_WhenTypeMatches()
        {
            // Arrange
            var paymentSetDto = new PaymentSetDto();
            Environment.SetEnvironmentVariable("TEST_BOOL", "true");
            
            // Act
            EnvironmentVariablesHelper.SetValueFromEnvVar<PaymentSetDto, bool>(
                paymentSetDto,
                dto => dto.InvoicesAttached,
                "TEST_BOOL");
            
            // Assert
            paymentSetDto.InvoicesAttached.Should().BeTrue();
        }

        [TestMethod]
        public void SetValueFromEnvVar_ShouldThrow_WhenInvalidType()
        {
            // Arrange
            var paymentSetDto = new PaymentSetDto();
            Environment.SetEnvironmentVariable("TEST_X", "true");
            
            // Act
            Action act = () =>
                EnvironmentVariablesHelper.SetValueFromEnvVar<PaymentSetDto, bool>(
                    paymentSetDto,
                    dto => dto.ForMonth,
                    "TEST_X");
            
            // Assert
            act.Should().Throw<Exception>();
        }

        [TestMethod]
        public void SetValueFromEnvVar_ShouldNotSetValue_WhenEnvVarMissing()
        {
            // Arrange
            var tokenDto = new TokenDto();

            // Act
            EnvironmentVariablesHelper.SetValueFromEnvVar<TokenDto, bool>(
                tokenDto,
                dto => dto.Token,
                "SOME_VAR");
            
            // Assert
            tokenDto.Token.Should().BeNullOrWhiteSpace();
        }
    }
}