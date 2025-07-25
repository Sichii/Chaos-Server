#region
using Chaos.Extensions.Common;
#endregion

namespace ChaosTool.ViewModel.Abstractions;

public class SchemaViewModelBase<TSchema> : ViewModelBase where TSchema: class
{
    /// <inheritdoc />
    public override async void AcceptChanges()
    {
        try
        {
            if (!IsChanged)
                return;

            var repository = JsonContext.GetRepository<TSchema>();

            if (!IsInserted)
                repository.Remove(OriginalPath);

            if (IsDeleted)
                return;

            var schema = this.MapTo<TSchema>();
            var wrapped = repository.Add(Path, schema);

            await repository.SaveItemAsync(wrapped);
            OriginalPath = Path;

            IsDeleted = false;
            IsInserted = false;
            IsChanged = false;
        } catch
        {
            //ignored
        }
    }

    /// <inheritdoc />
    public override void RejectChanges()
    {
        var repository = JsonContext.GetRepository<TSchema>();

        //if we reject on a freshly inserted record, it just gets set to default
        if (IsInserted)
        {
            var type = GetType();
            var @new = Activator.CreateInstance(type);

            @new!.MapPropertiesToInstance(this);
        } else
        {
            var wrapped = repository.Objects.FirstOrDefault(obj => obj.Path.EqualsI(OriginalPath));

            if (wrapped is not null)
            {
                var schema = wrapped.Object;

                schema.MapPropertiesToInstance(this);
            }
        }

        Path = OriginalPath;

        IsDeleted = false;

        //if we revert on a freshly inserted record, it just becomes blank, but is still freshly inserted
        //IsInserted = false;
        IsChanged = false;
    }
}