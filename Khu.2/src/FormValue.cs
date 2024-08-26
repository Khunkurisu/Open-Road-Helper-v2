namespace OpenRoadHelper
{
    public struct FormValue
    {
        public string Context;
        public ulong User;
        public string Target;
        public string Modifier;

        public FormValue()
        {
            Context = string.Empty;
            User = 0;
            Target = string.Empty;
            Modifier = string.Empty;
        }

        public FormValue(string context, ulong user, string target, string modifier)
        {
            Context = context ?? string.Empty;
            User = user;
            Target = target ?? string.Empty;
            Modifier = modifier ?? string.Empty;
        }

        public FormValue(List<dynamic> values)
        {
            Context = string.Empty;
            User = 0;
            Target = string.Empty;
            Modifier = string.Empty;
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
        }
    }
}
