import React, { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import { api } from '../lib/api';
import { Time, getApiErrorMessage } from '../types';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Switch } from './ui/switch';
import { Loader2, Save, X, Building2 } from 'lucide-react';
import { toast } from 'sonner';
import UploadFoto from './UploadFoto';

interface EditarPerfilTimeProps {
  time: Time;
  onClose: () => void;
  onSuccess: (timeAtualizado: Time) => void;
}

const EditarPerfilTime: React.FC<EditarPerfilTimeProps> = ({
  time,
  onClose,
  onSuccess
}) => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    nome: time.nome || '',
    pais: time.pais || 'BR',
    tier: time.tier || 1,
    contratando: time.contratando || false,
    logoUrl: time.logoUrl || ''
  });

  type TimeFormData = typeof formData;

  const handleInputChange = <K extends keyof TimeFormData>(field: K, value: TimeFormData[K]) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      const dadosAtualizados = await api.times.atualizarTime(formData);
      onSuccess(dadosAtualizados);
      toast.success('Time atualizado com sucesso!');
      onClose();
    } catch (error: unknown) {
      toast.error(getApiErrorMessage(error, 'Erro ao atualizar time'));
    } finally {
      setLoading(false);
    }
  };

  const handleLogoUpload = (url: string) => {
    setFormData(prev => ({
      ...prev,
      logoUrl: url
    }));
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
      <Card className="w-full max-w-2xl max-h-[90vh] overflow-y-auto">
        <CardHeader>
          <div className="flex items-center justify-between">
            <div>
              <CardTitle className="flex items-center gap-2">
                <Building2 className="h-5 w-5" />
                Editar Time
              </CardTitle>
              <CardDescription>
                Atualize as informações do seu time
              </CardDescription>
            </div>
            <Button variant="ghost" size="sm" onClick={onClose}>
              <X className="h-4 w-4" />
            </Button>
          </div>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Logo do Time */}
            <div className="space-y-2">
              <Label>Logo do Time</Label>
              <UploadFoto
                onUpload={handleLogoUpload}
                currentUrl={formData.logoUrl}
                disabled={loading}
              />
            </div>

            {/* Informações Básicas */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="nome">Nome do Time *</Label>
                <Input
                  id="nome"
                  value={formData.nome}
                  onChange={(e) => handleInputChange('nome', e.target.value)}
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
                <Label htmlFor="tier">Tier</Label>
                <Select value={formData.tier.toString()} onValueChange={(value) => handleInputChange('tier', parseInt(value))}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="1">Tier 1</SelectItem>
                    <SelectItem value="2">Tier 2</SelectItem>
                    <SelectItem value="3">Tier 3</SelectItem>
                    <SelectItem value="4">Tier 4</SelectItem>
                    <SelectItem value="5">Tier 5</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label>Status de Contratação</Label>
                <div className="flex items-center space-x-2">
                  <Switch
                    checked={formData.contratando}
                    onCheckedChange={(checked) => handleInputChange('contratando', checked)}
                  />
                  <Label className="text-sm">
                    {formData.contratando ? 'Contratando jogadores' : 'Não contratando'}
                  </Label>
                </div>
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

export default EditarPerfilTime;
