import { PaymentResponseDto } from './payment_response_dto';

export interface UserResponseDto {
  email: string;
  username: string;
  picture: string;
  remainingDocuments: string;
  remainingQuestions: string;
  payments: PaymentResponseDto[];
}
