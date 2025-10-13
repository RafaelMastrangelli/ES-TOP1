import { 
  Jogador, 
  JogadoresPaginados, 
  FiltrosJogadores, 
  Time, 
  CriarJogadorRequest, 
  CriarTimeRequest, 
  CriarInscricaoRequest,
  ApiResponse,
  FaceitPlayer,
  FaceitStats,
  FaceitMatch
} from '@/types';

const API_BASE_URL = 'http://localhost:5059/api';

// Helper para fazer requisições HTTP
const request = async <T>(
  url: string, 
  options: RequestInit = {}
): Promise<T> => {
  const response = await fetch(url, {
    headers: {
      'Content-Type': 'application/json',
      ...options.headers,
    },
    ...options,
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Erro ${response.status}: ${errorText}`);
  }

  return response.json();
};

export const api = {
  jogadores: {
    listar: async (filtros?: FiltrosJogadores): Promise<JogadoresPaginados> => {
      const params = new URLSearchParams();
      
      if (filtros?.q) params.append('q', filtros.q);
      if (filtros?.pais) params.append('pais', filtros.pais);
      if (filtros?.status) params.append('status', filtros.status);
      if (filtros?.disp) params.append('disp', filtros.disp);
      if (filtros?.funcao) params.append('funcao', filtros.funcao);
      if (filtros?.maxIdade) params.append('maxIdade', filtros.maxIdade.toString());
      if (filtros?.ordenar) params.append('ordenar', filtros.ordenar);
      if (filtros?.page) params.append('page', filtros.page.toString());
      if (filtros?.pageSize) params.append('pageSize', filtros.pageSize.toString());

      return request<JogadoresPaginados>(`${API_BASE_URL}/jogadores?${params}`);
    },

    buscarPorId: async (id: string): Promise<Jogador> => {
      return request<Jogador>(`${API_BASE_URL}/jogadores/${id}`);
    },

    criar: async (dados: CriarJogadorRequest): Promise<Jogador> => {
      return request<Jogador>(`${API_BASE_URL}/jogadores`, {
        method: 'POST',
        body: JSON.stringify(dados),
      });
    },

    alterarVisibilidade: async (id: string, visivel: boolean): Promise<ApiResponse<null>> => {
      return request<ApiResponse<null>>(`${API_BASE_URL}/jogadores/${id}/visibilidade?on=${visivel}`, {
        method: 'PUT',
      });
    },
  },

  times: {
    listar: async (): Promise<Time[]> => {
      return request<Time[]>(`${API_BASE_URL}/times`);
    },

    buscarPorId: async (id: string): Promise<Time> => {
      return request<Time>(`${API_BASE_URL}/times/${id}`);
    },

    criar: async (dados: CriarTimeRequest): Promise<Time> => {
      return request<Time>(`${API_BASE_URL}/times`, {
        method: 'POST',
        body: JSON.stringify(dados),
      });
    },
  },

  inscricoes: {
    criar: async (dados: CriarInscricaoRequest): Promise<ApiResponse<{ inscricaoId: string }>> => {
      return request<ApiResponse<{ inscricaoId: string }>>(`${API_BASE_URL}/inscricoes`, {
        method: 'POST',
        body: JSON.stringify(dados),
      });
    },

    pagar: async (id: string): Promise<ApiResponse<null>> => {
      return request<ApiResponse<null>>(`${API_BASE_URL}/inscricoes/${id}/pagar`, {
        method: 'POST',
      });
    },

    aprovar: async (id: string): Promise<ApiResponse<null>> => {
      return request<ApiResponse<null>>(`${API_BASE_URL}/inscricoes/${id}/aprovar`, {
        method: 'POST',
      });
    },
  },

  faceit: {
    buscarJogador: async (nickname: string): Promise<FaceitPlayer> => {
      return request<FaceitPlayer>(`${API_BASE_URL}/integracoes/faceit/jogador/${encodeURIComponent(nickname)}`);
    },

    buscarEstatisticas: async (playerId: string): Promise<FaceitStats> => {
      return request<FaceitStats>(`${API_BASE_URL}/integracoes/faceit/estatisticas/${playerId}`);
    },

    buscarPartidas: async (playerId: string, limite: number = 5): Promise<FaceitMatch[]> => {
      return request<FaceitMatch[]>(`${API_BASE_URL}/integracoes/faceit/partidas/${playerId}?limite=${limite}`);
    },
  },
};
