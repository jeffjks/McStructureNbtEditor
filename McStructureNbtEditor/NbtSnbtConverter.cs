using fNbt;

public static class NbtSnbtConverter
{
    private const int IndentSize = 2;
    public static string ToSnbt(NbtTag tag, int indentDepth = 0)
    {
        return tag switch
        {
            NbtCompound compound => CompoundToSnbt(compound, indentDepth),
            NbtList list => ListToSnbt(list, indentDepth),

            NbtString s => $"\"{s.Value}\"",
            NbtInt i => i.Value.ToString(),
            NbtLong l => $"{l.Value}L",
            NbtShort s => $"{s.Value}s",
            NbtByte b => $"{b.Value}b",
            NbtFloat f => $"{f.Value}f",
            NbtDouble d => $"{d.Value}d",

            _ => "null"
        };
    }

    private static string CompoundToSnbt(NbtCompound compound, int indentDepth)
    {
        if (compound.Tags.Count() == 0)
            return "{}";

        var indentStr = new string(' ', indentDepth * IndentSize);
        var childIndentStr = new string(' ', (indentDepth + 1) * IndentSize);

        var entries = compound.Tags.Select(t =>
        {
            var name = EscapeName(t.Name!);
            var value = ToSnbt(t, indentDepth + 1);
            return $"{childIndentStr}{name}: {value}";
        });

        return "{\n"
            + string.Join(",\n", entries)
            + "\n" + indentStr + "}";
    }

    private static string ListToSnbt(NbtList list, int indentDepth)
    {
        if (list.Count == 0)
            return "[]";

        var indentStr = new string(' ', indentDepth * IndentSize);
        var childIndentStr = new string(' ', (indentDepth + 1) * IndentSize);

        var items = list.Select(t =>
            $"{childIndentStr}{ToSnbt(t, indentDepth + 1)}"
        );

        return "[\n"
            + string.Join(",\n", items)
            + "\n" + indentStr + "]";
    }

    private static string EscapeName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "\"\"";

        if (name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-'))
            return name;

        return $"\"{name}\"";
    }
}