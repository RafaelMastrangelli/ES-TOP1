import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { PublicLayout } from "@/components/PublicLayout";
import { ProtectedLayout } from "@/components/ProtectedLayout";
import { 
  ProtectedRoute, 
  PublicOnlyRoute, 
  OrganizationRoute, 
  PlayerRoute 
} from "@/components/AuthGuard";
import Index from "./pages/Index";
import Jogadores from "./pages/Jogadores";
import JogadorDetalhes from "./pages/JogadorDetalhes";
import Times from "./pages/Times";
import TimeDetalhes from "./pages/TimeDetalhes";
import Login from "./pages/Login";
import Cadastro from "./pages/Cadastro";
import Inscricao from "./pages/Inscricao";
import InscricaoSucesso from "./pages/InscricaoSucesso";
import Assinaturas from "./pages/Assinaturas";
import Pagamento from "./pages/Pagamento";
import PagamentoSucesso from "./pages/PagamentoSucesso";
import Perfil from "./pages/Perfil";
import NotFound from "./pages/NotFound";

const queryClient = new QueryClient();

const App = () => (
  <QueryClientProvider client={queryClient}>
    <TooltipProvider>
      <Toaster />
      <Sonner />
      <BrowserRouter>
        <Routes>
          {/* Páginas públicas - acessíveis sem autenticação */}
          <Route path="/" element={
            <PublicLayout>
              <Index />
            </PublicLayout>
          } />
          
          <Route path="/login" element={
            <PublicOnlyRoute>
              <PublicLayout>
                <Login />
              </PublicLayout>
            </PublicOnlyRoute>
          } />
          
          <Route path="/cadastro" element={
            <PublicOnlyRoute>
              <PublicLayout>
                <Cadastro />
              </PublicLayout>
            </PublicOnlyRoute>
          } />
          
          <Route path="/inscricao" element={
            <PublicOnlyRoute>
              <PublicLayout>
                <Inscricao />
              </PublicLayout>
            </PublicOnlyRoute>
          } />
          
          <Route path="/inscricao-sucesso" element={
            <PublicOnlyRoute>
              <PublicLayout>
                <InscricaoSucesso />
              </PublicLayout>
            </PublicOnlyRoute>
          } />

          {/* Páginas protegidas - requerem autenticação */}
          <Route path="/jogadores" element={
            <OrganizationRoute>
              <ProtectedLayout>
                <Jogadores />
              </ProtectedLayout>
            </OrganizationRoute>
          } />
          
          <Route path="/jogadores/:id" element={
            <OrganizationRoute>
              <ProtectedLayout>
                <JogadorDetalhes />
              </ProtectedLayout>
            </OrganizationRoute>
          } />
          
          <Route path="/times" element={
            <ProtectedRoute>
              <ProtectedLayout>
                <Times />
              </ProtectedLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/times/:id" element={
            <ProtectedRoute>
              <ProtectedLayout>
                <TimeDetalhes />
              </ProtectedLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/assinaturas" element={
            <ProtectedRoute>
              <ProtectedLayout>
                <Assinaturas />
              </ProtectedLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/pagamento" element={
            <ProtectedRoute>
              <ProtectedLayout>
                <Pagamento />
              </ProtectedLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/pagamento-sucesso" element={
            <ProtectedRoute>
              <ProtectedLayout>
                <PagamentoSucesso />
              </ProtectedLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/perfil" element={
            <ProtectedRoute>
              <ProtectedLayout>
                <Perfil />
              </ProtectedLayout>
            </ProtectedRoute>
          } />

          {/* Página 404 */}
          <Route path="*" element={
            <PublicLayout>
              <NotFound />
            </PublicLayout>
          } />
        </Routes>
      </BrowserRouter>
    </TooltipProvider>
  </QueryClientProvider>
);

export default App;
