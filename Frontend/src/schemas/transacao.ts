import { z } from 'zod'

export const transacaoSchema = z.object({
  descricao: z
    .string()
    .min(1, 'Descrição é obrigatória')
    .max(400, 'Descrição deve ter no máximo 400 caracteres'),
  valor: z
    .number({ invalid_type_error: 'Valor deve ser um número' })
    .positive('Valor deve ser maior que 0'),
  tipo: z.coerce.number().int().min(0).max(1),
  categoriaId: z.string().uuid('Selecione uma categoria'),
  pessoaId: z.string().uuid('Selecione uma pessoa'),
})

export type TransacaoFormData = z.infer<typeof transacaoSchema>
