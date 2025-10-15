import { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useMutation } from '@tanstack/react-query';
import { api } from '@/lib/api';
import { CriarInscricaoRequest } from '@/types';
import AuthForm from '@/components/AuthForm';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Target, Check } from 'lucide-react';
import LoadingSpinner from '@/components/ui/loading-spinner';
import { useAuth } from '@/hooks/useAuth';

const Inscricao = () => {
  const [formData, setFormData] = useState<CriarInscricaoRequest>({
    apelido: '',
    pais: 'BR',
    idade: 18,
    funcaoPrincipal: 'Entry',
    rating: undefined,
    kd: undefined,
    partidasJogadas: undefined
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  // Redirecionar se já estiver autenticado
  useEffect(() => {
    if (isAuthenticated) {
      navigate('/', { replace: true });
    }
  }, [isAuthenticated, navigate]);

  const handleChange = (field: keyof CriarInscricaoRequest, value: any) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
  };

  const mutation = useMutation({
    mutationFn: (dados: CriarInscricaoRequest) => api.inscricoes.criar(dados),
    onSuccess: (response) => {
      navigate('/inscricao-sucesso', { 
        state: { inscricaoId: response.data?.inscricaoId || 'unknown' } 
      });
    },
    onError: (error: Error) => {
      setError(error.message);
      setIsLoading(false);
    }
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);
    
    // Validações básicas
    if (!formData.apelido.trim()) {
      setError('Apelido é obrigatório');
      setIsLoading(false);
      return;
    }
    
    if (formData.idade < 16 || formData.idade > 50) {
      setError('Idade deve estar entre 16 e 50 anos');
      setIsLoading(false);
      return;
    }

    mutation.mutate(formData);
  };

  const funcoes = [
    { value: 'Entry', label: 'Entry Fragger' },
    { value: 'Suporte', label: 'Suporte' },
    { value: 'Awp', label: 'AWP' },
    { value: 'Igl', label: 'IGL (In-Game Leader)' },
    { value: 'Lurker', label: 'Lurker' }
  ];

  const paises = [
    { value: 'BR', label: '🇧🇷 Brasil' },
    { value: 'US', label: '🇺🇸 Estados Unidos' },
    { value: 'EU', label: '🇪🇺 Europa' },
    { value: 'AR', label: '🇦🇷 Argentina' },
    { value: 'CL', label: '🇨🇱 Chile' }
  ];

  // Não renderizar se já estiver autenticado
  if (isAuthenticated) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <LoadingSpinner size="lg" />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-background">
      
      <AuthForm 
        title="Inscreva-se como Aspirante"
        description="Cadastre-se na plataforma e seja descoberto por times profissionais"
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          {error && (
            <div className="p-3 text-sm text-destructive bg-destructive/10 border border-destructive/20 rounded-md">
              {error}
            </div>
          )}

          <div className="space-y-2">
            <Label htmlFor="apelido">Apelido no jogo *</Label>
            <Input
              id="apelido"
              value={formData.apelido}
              onChange={(e) => handleChange('apelido', e.target.value)}
              placeholder="Seu apelido no CS2"
              required
              className="bg-input border-input"
            />
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="pais">País *</Label>
              <Select value={formData.pais} onValueChange={(value) => handleChange('pais', value)}>
                <SelectTrigger className="bg-input border-input">
                  <SelectValue placeholder="Selecione seu país" />
                </SelectTrigger>
                <SelectContent>
                  {paises.map((pais) => (
                    <SelectItem key={pais.value} value={pais.value}>
                      {pais.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label htmlFor="idade">Idade *</Label>
              <Input
                id="idade"
                type="number"
                min="16"
                max="50"
                value={formData.idade}
                onChange={(e) => handleChange('idade', parseInt(e.target.value))}
                required
                className="bg-input border-input"
              />
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="funcao">Função Principal *</Label>
            <Select value={formData.funcaoPrincipal} onValueChange={(value) => handleChange('funcaoPrincipal', value)}>
              <SelectTrigger className="bg-input border-input">
                <SelectValue placeholder="Selecione sua função" />
              </SelectTrigger>
              <SelectContent>
                {funcoes.map((funcao) => (
                  <SelectItem key={funcao.value} value={funcao.value}>
                    {funcao.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-4">
            <div className="border-t pt-4">
              <h3 className="text-lg font-semibold mb-3">Estatísticas (Opcional)</h3>
              <p className="text-sm text-muted-foreground mb-4">
                Forneça suas estatísticas para aumentar suas chances de ser notado
              </p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div className="space-y-2">
                <Label htmlFor="rating">Rating</Label>
                <Input
                  id="rating"
                  type="number"
                  step="0.01"
                  min="0"
                  value={formData.rating || ''}
                  onChange={(e) => handleChange('rating', e.target.value ? parseFloat(e.target.value) : undefined)}
                  placeholder="Ex: 2.5"
                  className="bg-input border-input"
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="kd">K/D Ratio</Label>
                <Input
                  id="kd"
                  type="number"
                  step="0.01"
                  min="0"
                  value={formData.kd || ''}
                  onChange={(e) => handleChange('kd', e.target.value ? parseFloat(e.target.value) : undefined)}
                  placeholder="Ex: 1.2"
                  className="bg-input border-input"
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="partidas">Partidas Jogadas</Label>
                <Input
                  id="partidas"
                  type="number"
                  min="0"
                  value={formData.partidasJogadas || ''}
                  onChange={(e) => handleChange('partidasJogadas', e.target.value ? parseInt(e.target.value) : undefined)}
                  placeholder="Ex: 150"
                  className="bg-input border-input"
                />
              </div>
            </div>
          </div>

          <div className="space-y-4">
            <div className="flex items-start space-x-2">
              <input
                id="aceiteTermos"
                type="checkbox"
                required
                className="mt-1 rounded border-input"
              />
              <Label htmlFor="aceiteTermos" className="text-sm leading-relaxed">
                Eu aceito os{' '}
                <a href="/termos" className="text-primary hover:text-primary/80 transition-colors">
                  Termos de Uso
                </a>
                {' '}e a{' '}
                <a href="/privacidade" className="text-primary hover:text-primary/80 transition-colors">
                  Política de Privacidade
                </a>
                {' '}da plataforma
              </Label>
            </div>
          </div>

          <Button 
            type="submit" 
            className="w-full" 
            disabled={isLoading}
          >
            {isLoading ? (
              <>
                <LoadingSpinner size="sm" className="mr-2" />
                Criando inscrição...
              </>
            ) : (
              'Inscrever-se'
            )}
          </Button>
        </form>

        <div className="mt-6 text-center">
          <p className="text-sm text-muted-foreground">
            Já tem uma conta?{' '}
            <Link 
              to="/login" 
              className="text-primary hover:text-primary/80 transition-colors font-medium"
            >
              Faça login
            </Link>
          </p>
        </div>
      </AuthForm>
    </div>
  );
};

export default Inscricao;
