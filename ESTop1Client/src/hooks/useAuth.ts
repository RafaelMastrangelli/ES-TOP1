import { useState, useEffect } from 'react';

interface User {
  id: string;
  nome: string;
  email: string;
}

interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}

export const useAuth = () => {
  const [authState, setAuthState] = useState<AuthState>({
    user: null,
    isAuthenticated: false,
    isLoading: true
  });

  useEffect(() => {
    // Simular verificação de autenticação
    const checkAuth = async () => {
      try {
        // Aqui você faria a verificação real com a API
        const token = localStorage.getItem('auth_token');
        if (token) {
          // Simular dados do usuário
          setAuthState({
            user: {
              id: '1',
              nome: 'Usuário Teste',
              email: 'usuario@teste.com'
            },
            isAuthenticated: true,
            isLoading: false
          });
        } else {
          setAuthState({
            user: null,
            isAuthenticated: false,
            isLoading: false
          });
        }
      } catch (error) {
        setAuthState({
          user: null,
          isAuthenticated: false,
          isLoading: false
        });
      }
    };

    checkAuth();
  }, []);

  const login = async (email: string, password: string) => {
    try {
      // TODO: Implementar API real de autenticação
      // Por enquanto, simular login
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      // Validação básica (remover quando implementar API real)
      if (email === 'admin@estop1.com' && password === 'admin123') {
        const token = 'fake-jwt-token';
        localStorage.setItem('auth_token', token);
        
        setAuthState({
          user: {
            id: '1',
            nome: 'Administrador',
            email: email
          },
          isAuthenticated: true,
          isLoading: false
        });
        
        return { success: true };
      } else {
        return { success: false, error: 'Email ou senha incorretos' };
      }
    } catch (error) {
      return { success: false, error: 'Erro ao fazer login' };
    }
  };

  const register = async (nome: string, email: string, password: string) => {
    try {
      // TODO: Implementar API real de cadastro
      // Por enquanto, simular cadastro
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      // Validação básica (remover quando implementar API real)
      if (email.includes('@') && password.length >= 6) {
        const token = 'fake-jwt-token';
        localStorage.setItem('auth_token', token);
        
        setAuthState({
          user: {
            id: '1',
            nome: nome,
            email: email
          },
          isAuthenticated: true,
          isLoading: false
        });
        
        return { success: true };
      } else {
        return { success: false, error: 'Email inválido ou senha muito curta' };
      }
    } catch (error) {
      return { success: false, error: 'Erro ao criar conta' };
    }
  };

  const logout = () => {
    localStorage.removeItem('auth_token');
    setAuthState({
      user: null,
      isAuthenticated: false,
      isLoading: false
    });
  };

  return {
    ...authState,
    login,
    register,
    logout
  };
};
