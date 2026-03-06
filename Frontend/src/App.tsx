import { Routes, Route } from 'react-router-dom'
import Layout from './components/Layout'
import Home from './pages/Home/Home'
import PessoaList from './pages/Pessoas/PessoaList'
import PessoaForm from './pages/Pessoas/PessoaForm'
import CategoriaList from './pages/Categorias/CategoriaList'
import CategoriaForm from './pages/Categorias/CategoriaForm'
import TransacaoList from './pages/Transacoes/TransacaoList'
import TransacaoForm from './pages/Transacoes/TransacaoForm'
import TotaisPorPessoa from './pages/Dashboard/TotaisPorPessoa'
import TotaisPorCategoria from './pages/Dashboard/TotaisPorCategoria'

export default function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<Home />} />

        <Route path="/pessoas" element={<PessoaList />} />
        <Route path="/pessoas/novo" element={<PessoaForm />} />
        <Route path="/pessoas/:id/editar" element={<PessoaForm />} />

        <Route path="/categorias" element={<CategoriaList />} />
        <Route path="/categorias/novo" element={<CategoriaForm />} />
        <Route path="/categorias/:id/editar" element={<CategoriaForm />} />

        <Route path="/transacoes" element={<TransacaoList />} />
        <Route path="/transacoes/novo" element={<TransacaoForm />} />
        <Route path="/transacoes/:id/editar" element={<TransacaoForm />} />

        <Route path="/dashboard/pessoas" element={<TotaisPorPessoa />} />
        <Route path="/dashboard/categorias" element={<TotaisPorCategoria />} />
      </Routes>
    </Layout>
  )
}
