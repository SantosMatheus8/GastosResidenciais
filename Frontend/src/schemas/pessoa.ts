import { z } from 'zod'

export const pessoaSchema = z.object({
  nome: z
    .string()
    .min(1, 'Nome é obrigatório')
    .max(200, 'Nome deve ter no máximo 200 caracteres'),
  idade: z
    .number({ invalid_type_error: 'Idade deve ser um número' })
    .int('Idade deve ser inteira')
    .min(0, 'Idade deve ser maior ou igual a 0'),
})

export type PessoaFormData = z.infer<typeof pessoaSchema>
