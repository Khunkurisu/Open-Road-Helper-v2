namespace OpenRoadHelper
{
    public readonly struct FormValue
    {
        public readonly string Context;
        public readonly ulong User;
        public readonly string Target;
        public readonly string Modifier;
        public readonly List<string> MetaData;

        public FormValue()
        {
            Context = string.Empty;
            User = 0;
            Target = string.Empty;
            Modifier = string.Empty;
            MetaData = new();
        }

        public FormValue(
            string context,
            ulong user,
            string target,
            string modifier,
            List<string> metaData
        )
        {
            Context = context ?? string.Empty;
            User = user;
            Target = target ?? string.Empty;
            Modifier = modifier ?? string.Empty;
            MetaData = metaData;
        }

        public FormValue(List<dynamic> values)
        {
            Context = string.Empty;
            User = 0;
            Target = string.Empty;
            Modifier = string.Empty;
            MetaData = new();
            if (values.Any())
            {
                Context = values[0];
            }
            if (values.Count > 1)
            {
                User = values[1];
            }
            if (values.Count > 2)
            {
                Target = values[2];
            }
            if (values.Count > 3)
            {
                Modifier = values[3];
            }
            if (values.Count < 4)
            {
                MetaData = values[4];
            }
        }
    }
}
