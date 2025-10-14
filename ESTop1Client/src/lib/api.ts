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
  FaceitMatch,
  FaceitSearchResult
} from '@/types';

const API_BASE_URL = 'http://localhost:5059/api';

// Helper para fazer requisições HTTP
const request = async <T>(
  url: string, 
  options: RequestInit = {}
): Promise<T> => {
  const token = localStorage.getItem('auth_token');
  
  const response = await fetch(url, {
    headers: {
      'Content-Type': 'application/json',
      ...(token && { 'Authorization': `Bearer ${token}` }),
      ...options.headers,
    },
    ...options,
  });

  if (!response.ok) {
    let errorData;
    try {
      const responseText = await response.text();
      try {
        errorData = JSON.parse(responseText);
      } catch {
        errorData = { message: responseText };
      }
    } catch {
      errorData = { message: `Erro ${response.status}` };
    }
    
    const error = new Error(errorData.message || `Erro ${response.status}`);
    (error as any).response = { data: errorData, status: response.status };
    throw error;
  }

  return response.json();
};

export const api = {
  // Métodos HTTP básicos para usar com axios
  get: async (url: string) => {
    const token = localStorage.getItem('auth_token');
    const response = await fetch(`${API_BASE_URL}${url}`, {
      headers: {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` }),
      },
    });
    
    if (!response.ok) {
      let errorData;
      try {
        const responseText = await response.text();
        try {
          errorData = JSON.parse(responseText);
        } catch {
          errorData = { message: responseText };
        }
      } catch {
        errorData = { message: `Erro ${response.status}` };
      }
      
      const error = new Error(errorData.message || `Erro ${response.status}`);
      (error as any).response = { data: errorData, status: response.status };
      throw error;
    }
    
    return { data: await response.json() };
  },

  post: async (url: string, data?: any) => {
    const token = localStorage.getItem('auth_token');
    const response = await fetch(`${API_BASE_URL}${url}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` }),
      },
      body: data ? JSON.stringify(data) : undefined,
    });
    
    if (!response.ok) {
      let errorData;
      try {
        const responseText = await response.text();
        try {
          errorData = JSON.parse(responseText);
        } catch {
          errorData = { message: responseText };
        }
      } catch {
        errorData = { message: `Erro ${response.status}` };
      }
      
      const error = new Error(errorData.message || `Erro ${response.status}`);
      (error as any).response = { data: errorData, status: response.status };
      throw error;
    }
    
    return { data: await response.json() };
  },

  auth: {
    login: async (email: string, senha: string) => {
      return request<{
        token: string;
        usuario: any;
        assinatura: any;
      }>(`${API_BASE_URL}/auth/login`, {
        method: 'POST',
        body: JSON.stringify({ email, senha }),
      });
    },

    registro: async (nome: string, email: string, senha: string, tipo: string) => {
      return request<{
        token: string;
        usuario: any;
        assinatura: any;
      }>(`${API_BASE_URL}/auth/registro`, {
        method: 'POST',
        body: JSON.stringify({ nome, email, senha, tipo }),
      });
    },

    me: async () => {
      return request<{
        usuario: any;
        assinatura: any;
      }>(`${API_BASE_URL}/auth/me`);
    },
  },

  assinaturas: {
    obterPlanos: async () => {
      return request<any[]>(`${API_BASE_URL}/assinaturas/planos`);
    },

    obterMinha: async () => {
      return request<any>(`${API_BASE_URL}/assinaturas/minha`);
    },

    criar: async (plano: string) => {
      return request<any>(`${API_BASE_URL}/assinaturas/criar`, {
        method: 'POST',
        body: JSON.stringify({ plano }),
      });
    },

    cancelar: async (id: string) => {
      return request<any>(`${API_BASE_URL}/assinaturas/${id}/cancelar`, {
        method: 'POST',
      });
    },

    renovar: async (id: string) => {
      return request<any>(`${API_BASE_URL}/assinaturas/${id}/renovar`, {
        method: 'POST',
      });
    },
  },

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

    buscarDadosCompletos: async (nickname: string): Promise<FaceitSearchResult> => {
      try {
        // Buscar jogador pelo nickname
        const player = await request<FaceitPlayer>(`${API_BASE_URL}/integracoes/faceit/jogador/${encodeURIComponent(nickname)}`);
        
        if (!player) {
          return { player: null, stats: null, matches: [] };
        }

        // Buscar estatísticas e partidas em paralelo
        const [stats, matches] = await Promise.allSettled([
          request<FaceitStats>(`${API_BASE_URL}/integracoes/faceit/estatisticas/${player.playerId}`),
          request<FaceitMatch[]>(`${API_BASE_URL}/integracoes/faceit/partidas/${player.playerId}?limite=5`)
        ]);

        return {
          player,
          stats: stats.status === 'fulfilled' ? stats.value : null,
          matches: matches.status === 'fulfilled' ? matches.value : []
        };
      } catch (error) {
        console.error('Erro ao buscar dados completos FACEIT:', error);
        return { player: null, stats: null, matches: [] };
      }
    },
  },

  openai: {
    buscarJogadores: async (consulta: string): Promise<{
      jogadores: any[];
      total: number;
      consultaOriginal: string;
      consultaIA: string;
    }> => {
      return request<{
        jogadores: any[];
        total: number;
        consultaOriginal: string;
        consultaIA: string;
      }>(`${API_BASE_URL}/integracoes/openai/buscar-jogadores?consulta=${encodeURIComponent(consulta)}`);
    },

    // TESTE: Endpoint para desenvolvimento sem verificação de assinatura
    buscarJogadoresTeste: async (consulta: string): Promise<{
      jogadores: any[];
      total: number;
      consultaOriginal: string;
      consultaIA: string;
    }> => {
      return request<{
        jogadores: any[];
        total: number;
        consultaOriginal: string;
        consultaIA: string;
      }>(`${API_BASE_URL}/integracoes/openai/teste/buscar-jogadores?consulta=${encodeURIComponent(consulta)}`);
    },

    sugerirFiltros: async (descricao: string): Promise<{
      filtros: any;
      descricaoOriginal: string;
    }> => {
      return request<{
        filtros: any;
        descricaoOriginal: string;
      }>(`${API_BASE_URL}/integracoes/openai/sugerir-filtros?descricao=${encodeURIComponent(descricao)}`);
    },
  },
};
