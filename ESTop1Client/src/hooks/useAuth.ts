import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { api } from '../lib/api';

interface User {
  id: string;
  nome: string;
  email: string;
  tipo: string;
}

interface Assinatura {
  id: string;
  plano: string;
  status: string;
  dataInicio: string;
  dataFim: string;
  valorMensal: number;
}

interface AuthState {
  user: User | null;
  assinatura: Assinatura | null;
  isAuthenticated: boolean;
  isLoading: boolean;
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
          // Verificar se o token ainda é válido
          const response = await api.get('/auth/me');
          if (response.data) {
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
      const response = await api.post('/auth/login', { email, senha: password });
      
      if (response.data.token) {
        localStorage.setItem('auth_token', response.data.token);
        
        setAuthState({
          user: response.data.usuario,
          assinatura: response.data.assinatura,
          isAuthenticated: true,
          isLoading: false
        });
        
        // Redirecionar para a página que o usuário tentou acessar ou para a home
        const from = location.state?.from?.pathname || '/';
        navigate(from, { replace: true });
        
        return { success: true };
      } else {
        return { success: false, error: 'Resposta inválida do servidor' };
      }
    } catch (error: any) {
      const errorMessage = error.response?.data?.message || 'Erro ao fazer login';
      return { success: false, error: errorMessage };
    }
  };

  const register = async (nome: string, email: string, password: string, tipo: string = 'Jogador') => {
    try {
      const response = await api.post('/auth/registro', { 
        nome, 
        email, 
        senha: password, 
        tipo 
      });
      
      if (response.data.token) {
        localStorage.setItem('auth_token', response.data.token);
        
        setAuthState({
          user: response.data.usuario,
          assinatura: response.data.assinatura,
          isAuthenticated: true,
          isLoading: false
        });
        
        // Redirecionar para a home após registro
        navigate('/', { replace: true });
        
        return { success: true };
      } else {
        return { success: false, error: 'Resposta inválida do servidor' };
      }
    } catch (error: any) {
      const errorMessage = error.response?.data?.message || 'Erro ao criar conta';
      return { success: false, error: errorMessage };
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
    // Redirecionar para a home após logout
    navigate('/', { replace: true });
  };

  return {
    ...authState,
    login,
    register,
    logout
  };
};
