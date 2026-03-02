CREATE TABLE Pessoas (
    Id        UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWID(),
    Nome      NVARCHAR(200)     NOT NULL,
    Idade     INT               NOT NULL,
    
    CONSTRAINT PK_Pessoas PRIMARY KEY (Id),
    CONSTRAINT CK_Pessoas_Idade CHECK (Idade >= 0)
);

CREATE TABLE Categorias (
    Id         UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWID(),
    Descricao  NVARCHAR(400)     NOT NULL,
    Finalidade TINYINT           NOT NULL,
    
    CONSTRAINT PK_Categorias PRIMARY KEY (Id),
    CONSTRAINT CK_Categorias_Finalidade CHECK (Finalidade IN (0, 1, 2))
);

CREATE TABLE Transacoes (
    Id          UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWID(),
    Descricao   NVARCHAR(400)     NOT NULL,
    Valor       DECIMAL(18, 2)    NOT NULL,
    Tipo        TINYINT           NOT NULL,
    CategoriaId UNIQUEIDENTIFIER  NOT NULL,
    PessoaId    UNIQUEIDENTIFIER  NOT NULL,

    CONSTRAINT PK_Transacoes           PRIMARY KEY (Id),
    CONSTRAINT FK_Transacoes_Categoria FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id),
    CONSTRAINT FK_Transacoes_Pessoa    FOREIGN KEY (PessoaId)    REFERENCES Pessoas(Id) ON DELETE CASCADE,
    CONSTRAINT CK_Transacoes_Valor     CHECK (Valor > 0),
    CONSTRAINT CK_Transacoes_Tipo      CHECK (Tipo IN (0, 1))
);