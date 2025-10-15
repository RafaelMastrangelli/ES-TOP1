import React, { useState, useRef } from 'react';
import { Button } from './ui/button';
import { Upload, X, Loader2, Check } from 'lucide-react';
import { toast } from 'sonner';

interface UploadFotoProps {
  onUpload: (url: string) => void;
  currentUrl?: string;
  disabled?: boolean;
}

const UploadFoto: React.FC<UploadFotoProps> = ({
  onUpload,
  currentUrl,
  disabled = false
}) => {
  const [uploading, setUploading] = useState(false);
  const [preview, setPreview] = useState<string | null>(currentUrl || null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileSelect = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    // Validar tipo de arquivo
    if (!file.type.startsWith('image/')) {
      toast.error('Por favor, selecione apenas arquivos de imagem');
      return;
    }

    // Validar tamanho (máximo 5MB)
    if (file.size > 5 * 1024 * 1024) {
      toast.error('A imagem deve ter no máximo 5MB');
      return;
    }

    setUploading(true);

    try {
      // Criar preview local
      const reader = new FileReader();
      reader.onload = (e) => {
        setPreview(e.target?.result as string);
      };
      reader.readAsDataURL(file);

      // Simular upload (em uma implementação real, você faria upload para um serviço como AWS S3)
      await new Promise(resolve => setTimeout(resolve, 2000));
      
      // Gerar uma URL temporária (em produção, seria a URL real do serviço de upload)
      const mockUrl = `https://via.placeholder.com/200x200/6366f1/ffffff?text=${encodeURIComponent(file.name.split('.')[0])}`;
      
      onUpload(mockUrl);
      toast.success('Foto enviada com sucesso!');
    } catch (error) {
      toast.error('Erro ao enviar foto');
      setPreview(currentUrl || null);
    } finally {
      setUploading(false);
    }
  };

  const handleRemovePhoto = () => {
    setPreview(null);
    onUpload('');
    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-4">
        {/* Preview da foto */}
        <div className="w-20 h-20 rounded-full overflow-hidden bg-muted flex items-center justify-center border-2 border-dashed border-muted-foreground/25">
          {preview ? (
            <img
              src={preview}
              alt="Preview"
              className="w-full h-full object-cover"
            />
          ) : (
            <Upload className="h-8 w-8 text-muted-foreground" />
          )}
        </div>

        {/* Botões de ação */}
        <div className="flex flex-col gap-2">
          <input
            ref={fileInputRef}
            type="file"
            accept="image/*"
            onChange={handleFileSelect}
            className="hidden"
            disabled={disabled || uploading}
          />
          
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={() => fileInputRef.current?.click()}
            disabled={disabled || uploading}
          >
            {uploading ? (
              <>
                <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                Enviando...
              </>
            ) : (
              <>
                <Upload className="h-4 w-4 mr-2" />
                {preview ? 'Alterar Foto' : 'Selecionar Foto'}
              </>
            )}
          </Button>

          {preview && (
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={handleRemovePhoto}
              disabled={disabled || uploading}
            >
              <X className="h-4 w-4 mr-2" />
              Remover
            </Button>
          )}
        </div>
      </div>

      {/* Informações sobre o upload */}
      <div className="text-xs text-muted-foreground">
        <p>Formatos aceitos: JPG, PNG, GIF</p>
        <p>Tamanho máximo: 5MB</p>
        <p>Dimensões recomendadas: 200x200px</p>
      </div>
    </div>
  );
};

export default UploadFoto;
