using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Domain.Events.Product;
using Ambev.DeveloperEvaluation.ORM.Persistence;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products
{
    public class CreateProductHandlerTests
    {
        // Gera um DbContext InMemory
        private DefaultContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new DefaultContext(options);
        }

        private IMapper BuildMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CreateProductProfile());
            });
            return new Mapper(config);
        }

        [Fact(DisplayName = "CreateProductHandler: Sucesso ao criar produto válido")]
        public async Task Handle_ValidCommand_ShouldCreateProduct()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var mediator = Substitute.For<IMediator>();
            var logger = Substitute.For<ILogger<CreateProductHandler>>();
            var mapper = BuildMapper();

            var handler = new CreateProductHandler(context, mediator, mapper, logger);
            var command = TestData.CreateProductHandlerTestData.ValidCommand();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.Name.Should().Be(command.Name);
            result.Price.Should().Be(command.Price);

            // Verifica que o produto foi salvo no contexto
            var saved = await context.Products.SingleOrDefaultAsync(p => p.Id == result.Id);
            saved.Should().NotBeNull();
            saved!.Name.Should().Be(command.Name);
            saved.Price.Should().Be(command.Price);

            // Evento publicado
            await mediator.Received(1).Publish(Arg.Any<ProductCreatedEvent>(), Arg.Any<CancellationToken>());
        }

        [Fact(DisplayName = "CreateProductHandler: Deve lançar erro quando Nome vazio")]
        public async Task Handle_InvalidCommand_ShouldThrowArgumentException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var mediator = Substitute.For<IMediator>();
            var mapper = BuildMapper();
            var logger = Substitute.For<ILogger<CreateProductHandler>>();

            var handler = new CreateProductHandler(context, mediator, mapper, logger);
            var badCommand = TestData.CreateProductHandlerTestData.InvalidCommand_MissingName();

            // Act
            Func<Task> act = async () => await handler.Handle(badCommand, CancellationToken.None);

            // Assert: o construtor de Product lança ArgumentException quando Name está vazio
            await act
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("*nome do produto não pode ser vazio*");
        }
    }
}
