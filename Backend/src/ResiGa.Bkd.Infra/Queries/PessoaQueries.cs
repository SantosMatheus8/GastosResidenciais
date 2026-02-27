namespace ResiGa.Bkd.Infra.Queries;

public static class PessoaQueries
{
    public const string CreatePessoa = @"
           ";

    public const string ListPessoas = @"
";

    public const string Count = @"
      SELECT COUNT(*)
      FROM Pessoa r WITH(NOLOCK)
      WHERE 1=1";

    public const string FindPessoaById = @"
";

    public const string UpdatePessoa = @"
";

    public const string CreatePessoaTag = @"
   ";

    public const string DeletePessoaTags = @"
   ";

    public const string DeletePessoa = @"
  ";
}
