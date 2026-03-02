import { z } from 'zod'

export const categoriaSchema = z.object({
  descricao: z
    .string()
    .min(1, 'Descrição é obrigatória')
    .max(400, 'Descrição deve ter no máximo 400 caracteres'),
  finalidade: z.coerce
    .number()
    .int()
    .min(0, 'Finalidade inválida')
    .max(2, 'Finalidade inválida'),
})

export type CategoriaFormData = z.infer<typeof categoriaSchema>
