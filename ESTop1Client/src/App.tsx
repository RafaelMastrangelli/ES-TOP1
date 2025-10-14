import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route } from "react-router-dom";
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
import NotFound from "./pages/NotFound";

const queryClient = new QueryClient();

const App = () => (
  <QueryClientProvider client={queryClient}>
    <TooltipProvider>
      <Toaster />
      <Sonner />
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Index />} />
          <Route path="/jogadores" element={<Jogadores />} />
          <Route path="/jogadores/:id" element={<JogadorDetalhes />} />
          <Route path="/times" element={<Times />} />
          <Route path="/times/:id" element={<TimeDetalhes />} />
          <Route path="/login" element={<Login />} />
          <Route path="/cadastro" element={<Cadastro />} />
          <Route path="/inscricao" element={<Inscricao />} />
          <Route path="/inscricao-sucesso" element={<InscricaoSucesso />} />
          <Route path="/assinaturas" element={<Assinaturas />} />
          <Route path="/pagamento" element={<Pagamento />} />
          <Route path="/pagamento-sucesso" element={<PagamentoSucesso />} />
          {/* ADD ALL CUSTOM ROUTES ABOVE THE CATCH-ALL "*" ROUTE */}
          <Route path="*" element={<NotFound />} />
        </Routes>
      </BrowserRouter>
    </TooltipProvider>
  </QueryClientProvider>
);

export default App;
