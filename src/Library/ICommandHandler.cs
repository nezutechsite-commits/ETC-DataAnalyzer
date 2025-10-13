using Library.UseCase;

namespace Library;

public interface ICommandHandler<TCommand>
{
    CommandResult Handle(TCommand command);
}
