import React, { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import { api } from '../lib/api';
import { Jogador } from '../types';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Loader2, Save, X, User } from 'lucide-react';
import { toast } from 'sonner';
import UploadFoto from './UploadFoto';

interface EditarPerfilJogadorProps {
  jogador: Jogador;
  onClose: () => void;
  onSuccess: (jogadorAtualizado: Jogador) => void;
}

const EditarPerfilJogador: React.FC<EditarPerfilJogadorProps> = ({
  jogador,
  onClose,
  onSuccess
}) => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    apelido: jogador.apelido || '',
    pais: jogador.pais || 'BR',
    idade: jogador.idade || 18,
    funcaoPrincipal: jogador.funcaoPrincipal || 'Entry',
    status: jogador.status || 'Amador',
    disponibilidade: jogador.disponibilidade || 'Livre',
    valorDeMercado: jogador.valorDeMercado || 0,
    fotoUrl: jogador.fotoUrl || ''
  });

  const handleInputChange = (field: string, value: any) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      // Filtrar dados para enviar apenas campos válidos
      const dadosParaEnviar: any = {};
      
      if (formData.apelido && formData.apelido.trim()) {
        dadosParaEnviar.apelido = formData.apelido.trim();
      }
      if (formData.pais && formData.pais.trim()) {
        dadosParaEnviar.pais = formData.pais.trim();
      }
      if (formData.idade && formData.idade > 0) {
        dadosParaEnviar.idade = formData.idade;
      }
      if (formData.funcaoPrincipal) {
        dadosParaEnviar.funcaoPrincipal = formData.funcaoPrincipal;
      }
      if (formData.status) {
        dadosParaEnviar.status = formData.status;
      }
      if (formData.disponibilidade) {
        dadosParaEnviar.disponibilidade = formData.disponibilidade;
      }
      if (formData.valorDeMercado && formData.valorDeMercado >= 0) {
        dadosParaEnviar.valorDeMercado = formData.valorDeMercado;
      }
      if (formData.fotoUrl && formData.fotoUrl.trim()) {
        dadosParaEnviar.fotoUrl = formData.fotoUrl.trim();
      }

      const dadosAtualizados = await api.jogadores.atualizarPerfil(dadosParaEnviar);
      onSuccess(dadosAtualizados);
      toast.success('Perfil atualizado com sucesso!');
      onClose();
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Erro ao atualizar perfil');
    } finally {
      setLoading(false);
    }
  };

  const handleFotoUpload = (url: string) => {
    setFormData(prev => ({
      ...prev,
      fotoUrl: url
    }));
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
      <Card className="w-full max-w-2xl max-h-[90vh] overflow-y-auto">
        <CardHeader>
          <div className="flex items-center justify-between">
            <div>
              <CardTitle className="flex items-center gap-2">
                <User className="h-5 w-5" />
                Editar Perfil
              </CardTitle>
              <CardDescription>
                Atualize suas informações pessoais e de jogador
              </CardDescription>
            </div>
            <Button variant="ghost" size="sm" onClick={onClose}>
              <X className="h-4 w-4" />
            </Button>
          </div>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Foto do Perfil */}
            <div className="space-y-2">
              <Label>Foto do Perfil</Label>
              <UploadFoto
                onUpload={handleFotoUpload}
                currentUrl={formData.fotoUrl}
                disabled={loading}
              />
            </div>

            {/* Informações Básicas */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="apelido">Apelido *</Label>
                <Input
                  id="apelido"
                  value={formData.apelido}
                  onChange={(e) => handleInputChange('apelido', e.target.value)}
                  required
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="pais">País</Label>
                <Select value={formData.pais} onValueChange={(value) => handleInputChange('pais', value)}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="BR">🇧🇷 Brasil</SelectItem>
                    <SelectItem value="US">🇺🇸 Estados Unidos</SelectItem>
                    <SelectItem value="EU">🇪🇺 Europa</SelectItem>
                    <SelectItem value="AR">🇦🇷 Argentina</SelectItem>
                    <SelectItem value="CL">🇨🇱 Chile</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label htmlFor="idade">Idade</Label>
                <Input
                  id="idade"
                  type="number"
                  min="16"
                  max="50"
                  value={formData.idade}
                  onChange={(e) => handleInputChange('idade', e.target.value ? parseInt(e.target.value) : null)}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="valorDeMercado">Valor de Mercado (R$)</Label>
                <Input
                  id="valorDeMercado"
                  type="number"
                  min="0"
                  step="1000"
                  value={formData.valorDeMercado}
                  onChange={(e) => handleInputChange('valorDeMercado', e.target.value ? parseFloat(e.target.value) : null)}
                />
              </div>
            </div>

            {/* Informações de Jogador */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div className="space-y-2">
                <Label>Função Principal</Label>
                <Select value={formData.funcaoPrincipal} onValueChange={(value) => handleInputChange('funcaoPrincipal', value)}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Entry">Entry Fragger</SelectItem>
                    <SelectItem value="Suporte">Suporte</SelectItem>
                    <SelectItem value="Awp">AWP</SelectItem>
                    <SelectItem value="Igl">IGL</SelectItem>
                    <SelectItem value="Lurker">Lurker</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label>Status</Label>
                <Select value={formData.status} onValueChange={(value) => handleInputChange('status', value)}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Amador">Amador</SelectItem>
                    <SelectItem value="Profissional">Profissional</SelectItem>
                    <SelectItem value="Aposentado">Aposentado</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label>Disponibilidade</Label>
                <Select value={formData.disponibilidade} onValueChange={(value) => handleInputChange('disponibilidade', value)}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Livre">Livre</SelectItem>
                    <SelectItem value="EmTime">Em Time</SelectItem>
                    <SelectItem value="Teste">Teste</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>

            {/* Botões */}
            <div className="flex justify-end gap-3 pt-4">
              <Button type="button" variant="outline" onClick={onClose}>
                Cancelar
              </Button>
              <Button type="submit" disabled={loading}>
                {loading ? (
                  <>
                    <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                    Salvando...
                  </>
                ) : (
                  <>
                    <Save className="h-4 w-4 mr-2" />
                    Salvar Alterações
                  </>
                )}
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
};

export default EditarPerfilJogador;
