using Moq;
using SupplierPurchaseReport.Models;
using SupplierPurchaseReport.Repositories;
using SupplierPurchaseReport.Services;

namespace SupplierPurchaseReport.Tests
{
    public class SupplierReportServiceTests
    {
        [Fact]
        public async Task RunDailyReport_SendsEmail_WhenPurchasesExist()
        {
            // Arrange
            var mockEmailService = new Mock<IEmailService>();
            var mockPurchaseRepository = new Mock<IPurchaseRepository>();
            var mockCsvExportService = new Mock<ICsvExportService>();

            mockPurchaseRepository
                .Setup(r => r.GetBySupplier(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Purchase>
                {
                    new Purchase
                    {
                        Name = "Test Item",
                        Code = "001",
                        Branchnr = "1",
                        BranchName = "Test Branch",
                        DatePurchased = DateTime.Now,
                        Amount = 100
                    }
                });

            var service = new SupplierReportService(
                mockPurchaseRepository.Object,
                mockCsvExportService.Object,
                mockEmailService.Object,
                new Mock<Microsoft.Extensions.Logging.ILogger<SupplierReportService>>().Object
            );

            // Act
            await service.RunDailyReport("TestSupplier", 6, 2026, "test@test.com");

            // Assert
            mockEmailService.Verify(
                e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>()),
                Times.Once);
        }

        [Fact]
        public async Task RunDailyReport_DoesNotSendEmail_WhenPurchasesEmpty()
        {
            // Arrange
            var mockEmailService = new Mock<IEmailService>();
            var mockPurchaseRepository = new Mock<IPurchaseRepository>();
            var mockCsvExportService = new Mock<ICsvExportService>();

            mockPurchaseRepository
                .Setup(r => r.GetBySupplier(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Purchase>()
               );

            var service = new SupplierReportService(
                mockPurchaseRepository.Object,
                mockCsvExportService.Object,
                mockEmailService.Object,
                new Mock<Microsoft.Extensions.Logging.ILogger<SupplierReportService>>().Object
            );

            // Act
            await service.RunDailyReport("TestSupplier", 6, 2026, "test@test.com");

            // Assert
            mockEmailService.Verify(
                e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>()),
                Times.Never);
        }
        [Fact]
        public async Task RunDailyReport_DoesNotSendEmail_WhenExceptionThrown()
        {
            // Arrange
            var mockEmailService = new Mock<IEmailService>();
            var mockPurchaseRepository = new Mock<IPurchaseRepository>();
            var mockCsvExportService = new Mock<ICsvExportService>();

            mockPurchaseRepository
                .Setup(r => r.GetBySupplier(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database error")
                );

            var service = new SupplierReportService(
                mockPurchaseRepository.Object,
                mockCsvExportService.Object,
                mockEmailService.Object,
                new Mock<Microsoft.Extensions.Logging.ILogger<SupplierReportService>>().Object
            );

            // Act
            await service.RunDailyReport("TestSupplier", 6, 2026, "test@test.com");

            // Assert
            mockEmailService.Verify(
                e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>()),
                Times.Never);
        }
    }
}