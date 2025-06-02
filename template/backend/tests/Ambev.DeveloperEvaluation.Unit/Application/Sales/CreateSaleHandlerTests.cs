using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sale;
using Ambev.DeveloperEvaluation.ORM.Persistence;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class CreateSaleHandlerTests
    {
        private DefaultContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                // Suprime o aviso de transações não suportadas pelo InMemory
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var ctx = new DefaultContext(options);

            // Seed de produtos para que os IDs usados no teste existam
            ctx.Products.AddRange(
                new Product
                {
                    Id = Guid.Parse("6f1e4f02-1a35-4b2d-9fdd-3a7c5ab1ae2b"),
                    Name = "Produto A",
                    Price = 10m
                },
                new Product
                {
                    Id = Guid.Parse("b254a3d4-7c76-4e14-af08-2dcf928d3b10"),
                    Name = "Produto B",
                    Price = 20m
                }
            );
            ctx.SaveChanges();

            return ctx;
        }

        private IMapper BuildMapper()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile(new CreateSaleProfile()));
            return new Mapper(cfg);
        }

        [Fact(DisplayName = "CreateSaleHandler: Sucesso ao criar venda válida")]
        public async Task Handle_ValidCommand_ShouldCreateSale()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var mediator = Substitute.For<IMediator>();
            var logger = Substitute.For<ILogger<CreateSaleHandler>>();
            var mapper = BuildMapper();
            var handler = new CreateSaleHandler(context, mediator, mapper, logger);

            // Use os mesmos IDs de produto que foram seedados
            var pid1 = Guid.Parse("6f1e4f02-1a35-4b2d-9fdd-3a7c5ab1ae2b");
            var pid2 = Guid.Parse("b254a3d4-7c76-4e14-af08-2dcf928d3b10");

            var command = new CreateSaleCommand(
                IdempotencyKey: Guid.NewGuid().ToString(),
                CustomerId: Guid.NewGuid(),
                Items: new List<CreateSaleItemDto>
                {
                    new CreateSaleItemDto(pid1, 2),
                    new CreateSaleItemDto(pid2, 1)
                }
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);

            // Confirma que a venda foi salva em Sales
            var savedSale = await context.Sales
                .Include(s => s.Items)
                .SingleOrDefaultAsync(s => s.Id == result.Id);
            savedSale.Should().NotBeNull();
            savedSale!.Items.Should().HaveCount(2);

            // Evento publicado
            await mediator.Received(1).Publish(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
        }

        [Fact(DisplayName = "CreateSaleHandler: Deve lançar erro ao exceder 20 itens do mesmo produto")]
        public async Task Handle_TooManySameItems_ShouldThrowDomainException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var mapper = BuildMapper();
            var logger = Substitute.For<ILogger<CreateSaleHandler>>();
            var handler = new CreateSaleHandler(context, Substitute.For<IMediator>(), mapper, logger);

            // Este comando tenta adicionar 21 unidades do mesmo produto
            var pid = Guid.Parse("6f1e4f02-1a35-4b2d-9fdd-3a7c5ab1ae2b");
            var items = new List<CreateSaleItemDto>();
            for (int i = 0; i < 21; i++)
            {
                items.Add(new CreateSaleItemDto(pid, 1));
            }

            var badCommand = new CreateSaleCommand(
                IdempotencyKey: Guid.NewGuid().ToString(),
                CustomerId: Guid.NewGuid(),
                Items: items
            );

            // Act
            Func<Task> act = async () => await handler.Handle(badCommand, CancellationToken.None);

            // Assert: espera DomainException com mensagem de limite de 20 itens
            await act
                .Should()
                .ThrowAsync<DomainException>()
                .WithMessage("*20 unidades do mesmo item*");
        }
    }
}
