import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { api } from '../lib/api';
import { AuthAssinatura, AuthMeResponse, AuthResponse, AuthUser, getApiErrorMessage } from '../types';

interface AuthState {
  user: AuthUser | null;
  assinatura: AuthAssinatura | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}

interface LocationState {
  from?: {
    pathname: string;
  };
}

export const useAuth = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [authState, setAuthState] = useState<AuthState>({
    user: null,
    assinatura: null,
    isAuthenticated: false,
    isLoading: true
  });

  useEffect(() => {
    const checkAuth = async () => {
      try {
        const token = localStorage.getItem('auth_token');
        if (token) {
          const response = await api.get<AuthMeResponse>('/auth/me');

          if (response.data?.usuario) {
            setAuthState({
              user: response.data.usuario,
              assinatura: response.data.assinatura,
              isAuthenticated: true,
              isLoading: false
            });
          } else {
            localStorage.removeItem('auth_token');
            setAuthState({
              user: null,
              assinatura: null,
              isAuthenticated: false,
              isLoading: false
            });
          }
        } else {
          setAuthState({
            user: null,
            assinatura: null,
            isAuthenticated: false,
            isLoading: false
          });
        }
      } catch (error) {
        console.log('Erro ao verificar autenticação:', error);
        localStorage.removeItem('auth_token');
        setAuthState({
          user: null,
          assinatura: null,
          isAuthenticated: false,
          isLoading: false
        });
      }
    };

    checkAuth();
  }, []);

  const login = async (email: string, password: string) => {
    try {
      const response = await api.post<AuthResponse>('/auth/login', { email, senha: password });

      if (response.data?.Token) {
        localStorage.setItem('auth_token', response.data.Token);

        setAuthState({
          user: response.data.Usuario,
          assinatura: response.data.Assinatura,
          isAuthenticated: true,
          isLoading: false
        });

        const from = (location.state as LocationState | null)?.from?.pathname || '/';
        navigate(from, { replace: true });

        return { success: true as const };
      }

      return { success: false as const, error: 'Resposta inválida do servidor' };
    } catch (error: unknown) {
      return { success: false as const, error: getApiErrorMessage(error, 'Erro ao fazer login') };
    }
  };

  const register = async (nome: string, email: string, password: string, tipo: string = 'Jogador') => {
    try {
      const response = await api.post<AuthResponse>('/auth/registro', {
        nome,
        email,
        senha: password,
        tipo
      });

      if (response.data?.Token) {
        localStorage.setItem('auth_token', response.data.Token);

        setAuthState({
          user: response.data.Usuario,
          assinatura: response.data.Assinatura,
          isAuthenticated: true,
          isLoading: false
        });

        navigate('/', { replace: true });

        return { success: true as const };
      }

      return { success: false as const, error: 'Resposta inválida do servidor' };
    } catch (error: unknown) {
      return { success: false as const, error: getApiErrorMessage(error, 'Erro ao criar conta') };
    }
  };

  const logout = () => {
    localStorage.removeItem('auth_token');
    setAuthState({
      user: null,
      assinatura: null,
      isAuthenticated: false,
      isLoading: false
    });
    navigate('/', { replace: true });
  };

  return {
    ...authState,
    login,
    register,
    logout
  };
};
