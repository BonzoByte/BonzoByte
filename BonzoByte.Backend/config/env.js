// config/env.js
import 'dotenv/config';
import { z } from 'zod';

const Env = z.object({
    NODE_ENV: z.enum(['development', 'test', 'production']).default('development'),
    PORT: z.coerce.number().default(5000),
    MONGO_URI: z.string().min(1),
    JWT_SECRET: z.string().min(16),
    JWT_EXPIRES_IN: z.string().default('1d'),
    JWT_VERIFICATION_EXPIRES_IN: z.string().default('1h'),
    EMAIL_USER: z.string().email().optional(),
    EMAIL_PASS: z.string().optional(),
    CORS_ORIGINS: z.string().default('http://localhost:4200'),
    BASE_URL: z.string().url().default('http://localhost:5000'),
    GOOGLE_CLIENT_ID: z.string().optional(),
    GOOGLE_CLIENT_SECRET: z.string().optional(),
    GOOGLE_CALLBACK_URL: z.string().url().optional(),
    FACEBOOK_CLIENT_ID: z.string().optional(),
    FACEBOOK_CLIENT_SECRET: z.string().optional(),
    FACEBOOK_CALLBACK_URL: z.string().url().optional()
});

const parsed = Env.safeParse(process.env);
if (!parsed.success) {
    console.error('Invalid ENV:', parsed.error.flatten().fieldErrors);
    process.exit(1);
}

export const env = parsed.data;
export const corsAllowlist = env.CORS_ORIGINS.split(',').map(s => s.trim());
