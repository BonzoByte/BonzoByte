import User from '../models/user.model.js';
import bcrypt from 'bcryptjs';
import crypto from 'node:crypto';
import jwt from 'jsonwebtoken';
import { generateToken, generateVerificationToken } from '../utils/generateToken.js';
import sendVerificationEmail from '../utils/sendVerificationEmail.js';
import sendResetPasswordEmail from '../utils/sendResetPasswordEmail.js';
import sendEmail from '../utils/sendEmail.js';
import moment from 'moment';
import asyncHandler from 'express-async-handler';
import transporter from '../utils/mailer.js';

// ✅ Helper funkcija za slanje odgovora s greškom
const handleError = (res, statusCode, message) => {
  return res.status(statusCode).json({ message });
};

// ✅ REGISTER
export const registerUser = async (req, res) => {
  try {
    const { name, email, password, country } = req.body;

    if (!name || !email || !password) {
      return handleError(res, 400, 'Sva polja su obavezna.');
    }

    const existingUser = await User.findOne({ email });

    if (existingUser) {
      if (!existingUser.password) {
        const salt = await bcrypt.genSalt(10);
        existingUser.password = await bcrypt.hash(password, salt);
        existingUser.name = existingUser.name || name;
        if (!existingUser.provider.includes('local')) {
          existingUser.provider.push('local');
        }
        await existingUser.save();

        const verificationToken = generateVerificationToken(existingUser._id);
        await sendVerificationEmail(existingUser.email, existingUser, verificationToken);

        return res.status(200).json({ message: 'Lozinka dodana. Verificiraj email.' });
      } else {
        return res.status(400).json({ message: 'Korisnik s tim emailom već postoji.' });
      }
    }

    const salt = await bcrypt.genSalt(10);
    const hashedPassword = await bcrypt.hash(password, salt);

    const user = await User.create({
      name,
      email,
      password,
      country,
      provider: ['local'],
      createdVia: 'manual',
      isUser: false,
    });

    await user.save();

    const verificationToken = generateVerificationToken(user._id);
    await sendVerificationEmail(user.email, user, verificationToken);
    return res.status(201).json({ message: 'Uspješna registracija. Verificiraj email.' });

  } catch (error) {
    console.error('Greška u registerUser:', error);
    res.status(500).json({ message: 'Nešto je pošlo po zlu kod registracije.' });
  }
};

// ✅ LOGIN
export const loginUser = async (req, res) => {
  try {
    const { identifier, password } = req.body;
    // 1. Validacija ulaza
    if (!identifier || !password) {
      return res.status(400).json({ message: 'Email ili username i lozinka su obavezni.' });
    }

    // 2. Nađi korisnika
    const user = await User.findOne({
      $or: [
        { email: identifier },
        { nickname: identifier },
      ],
    }).populate('country', 'countryShort countryFull');

    //console.log(user);

    if (!user) {
      return res.status(404).json({
        message: 'Korisnik s tim emailom ili nadimkom ne postoji.',
        reason: 'user_not_found'
      });
    }

    // 3. Ako nema lozinku (Google login)
    if (!user.password) {
      return res.status(400).json({
        message: 'Ovaj korisnik se registrirao putem Googlea. Prijavi se preko Googlea.',
      });
    }

    // 4. Provjera lozinke
    const isMatch = await bcrypt.compare(password, user.password);
    if (!isMatch) {
      return res.status(401).json({
        message: 'Neispravna lozinka.',
        offerPasswordReset: true,
      });
    }

    // 5. Provjera je li verificiran
    //console.log('User found:', user);
    console.log('user.isUser =', user.isUser);
    if (!user.isUser) {
      console.log("korisnik nije verificiran");
      res.status(403);
      return res.json({
        message: 'Email not verified.',
        offerResendVerification: true,
        userId: user._id,
      });
    }

    // ✅ 6. Ažuriraj login podatke
    user.lastLogin = new Date();
    user.isOnline = true;
    await user.save();

    // 7. Generiraj token
    const token = generateToken(user._id);

    // 8. Vrati podatke
    return res.status(200).json({
      message: 'Prijava uspješna!',
      token,
      user: {
        id: user._id,
        name: user.name,
        nickname: user.nickname,
        email: user.email,
        isAdmin: user.isAdmin,
        isOnline: user.isOnline,
        lastLogin: user.lastLogin,
        avatarUrl: user.avatarUrl || null,
        country: user.country || null,
      },
    });

  } catch (error) {
    console.error('[LOGIN ERROR]:', error);
    res.status(500).json({ message: 'Greška na serveru prilikom prijave.' });
  }
};

// ✅ LOGOUT
export const logoutUser = async (req, res) => {
  try {
    const userId = req.user.id;

    const user = await User.findById(userId);

    if (!user) {
      return res.status(404).json({ message: 'Korisnik nije pronađen.' });
    }

    user.isOnline = false;
    await user.save();

    return res.status(200).json({ message: 'Odjava uspješna.' });
  } catch (error) {
    console.error('[LOGOUT ERROR]:', error);
    res.status(500).json({ message: 'Greška prilikom odjave.' });
  }
};

// ✅ VERIFY EMAIL
export const verifyEmail = async (req, res) => {
  try {
    const { token } = req.body;

    const decoded = jwt.verify(token, process.env.JWT_SECRET);
    const user = await User.findById(decoded.id);

    if (!user) return handleError(res, 404, 'Korisnik nije pronađen.');

    user.isVerified = true;
    user.isUser = true;
    await user.save();

    res.status(200).json({ message: 'Email uspješno verificiran.' });

  } catch (error) {
    console.error('[VERIFY EMAIL ERROR]:', error);
    handleError(res, 400, 'Token nije ispravan ili je istekao.');
  }
};

// ✅ RESEND VERIFY EMAIL
export const resendVerificationEmail = async (req, res) => {
  try {
    console.log("start verification");
    const { email } = req.body;
    console.log('[RESEND] Email primljen:', email); // <-- sada ok

    const user = await User.findOne({ email });
    console.log('[RESEND] User pronađen:', user);

    if (!user) {
      return res.status(400).json({ message: 'Korisnik ne postoji.' });
    }

    if (user.isUser) {
      return res.status(400).json({ message: 'Račun je već verificiran.' });
    }

    const token = generateVerificationToken(user._id);
    console.log('[RESEND] Token generiran:', token);

    await sendVerificationEmail(user.email, user, token);
    console.log('[RESEND] Email poslan');

    return res.status(200).json({ message: 'Verifikacijski email ponovno poslan.' });

  } catch (error) {
    console.error('[RESEND VERIFICATION ERROR]:', error);
    res.status(500).json({ message: 'Greška prilikom slanja verifikacije.' });
  }
};

export async function login(req, res) {
  const { email, password } = req.body;

  try {
    const user = await User.findOne({ email });

    if (!user) {
      return res.status(400).json({ message: 'Neispravni email ili lozinka.' });
    }

    const isMatch = await bcrypt.compare(password, user.password);

    if (!isMatch) {
      return res.status(400).json({ message: 'Neispravni email ili lozinka.' });
    }

    const token = jwt.sign(
      { id: user._id, email: user.email, isUser: user.isUser, isAdmin: user.isAdmin },
      process.env.JWT_SECRET,
      { expiresIn: '7d' } // token vrijedi 7 dana
    );

    res.json({ token });

  } catch (error) {
    console.error('Greška u login:', error);
    res.status(500).json({ message: 'Greška prilikom prijave' });
  }
}

export async function forgotPassword(req, res) {
    const { email } = req.body;
    try {
        const user = await User.findOne({ email });
        if (!user) return res.status(404).json({ message: 'Korisnik s tom email adresom ne postoji' });

        const token = crypto.randomBytes(32).toString('hex');
        user.resetPasswordToken = token;
        user.resetPasswordExpires = Date.now() + 60 * 60 * 1000; // 1 sat
        await user.save();

        await sendResetPasswordEmail(email, token);
        return res.json({ message: 'Email za reset lozinke je poslan' });
    } catch (err) {
        console.error('Greška u forgotPassword:', err);
        return res.status(500).json({ message: 'Greška prilikom slanja emaila' });
    }
}

export async function resetPassword(req, res) {
    const { token, email, password } = req.body;
    if (!token || !email || !password) {
        return res.status(400).json({ message: 'Token, email i nova lozinka su obavezni.' });
    }

    try {
        const user = await User.findOne({
            email,
            resetPasswordToken: token,
            resetPasswordExpires: { $gt: Date.now() }
        });

        if (!user) {
            return res.status(400).json({ message: 'Neispravan ili istekao token.' });
        }

        user.password = password;                // pre-save će hashirati
        user.resetPasswordToken = undefined;
        user.resetPasswordExpires = undefined;
        user.tokenVersion = (user.tokenVersion ?? 0) + 1; // opcionalno: poništi stare refresh tokene
        await user.save();

        return res.json({ message: 'Lozinka uspješno promijenjena.' });
    } catch (error) {
        console.error(error);
        return res.status(500).json({ message: 'Došlo je do greške prilikom resetiranja lozinke.' });
    }
}

export const getCurrentUser = async (req, res) => {
  try {
    const user = await User.findById(req.user.id).populate('country', 'countryShort countryFull');

    if (!user) return res.status(404).json({ message: 'Korisnik nije pronađen.' });

    return res.status(200).json({
      user: {
        id: user._id,
        name: user.name,
        nickname: user.nickname,
        email: user.email,
        isAdmin: user.isAdmin,
        isOnline: user.isOnline,
        lastLogin: user.lastLogin,
        avatarUrl: user.avatarUrl || null,
        country: user.country || null,
      },
    });
  } catch (err) {
    console.error('Greška kod dohvaćanja korisnika:', err);
    res.status(500).json({ message: 'Greška na serveru.' });
  }
};

export const requestResetPassword = async (req, res) => {
    const { email } = req.body;
    try {
        const user = await User.findOne({ email });
        if (!user) return res.status(400).json({ message: 'Korisnik s ovim emailom ne postoji.' });

        const token = crypto.randomBytes(32).toString('hex');
        user.resetPasswordToken = token;
        user.resetPasswordExpires = Date.now() + 60 * 60 * 1000;
        await user.save();

        await sendResetPasswordEmail(user.email, token);
        return res.status(200).json({ message: 'Link za resetiranje lozinke poslan.' });
    } catch (error) {
        console.error(error);
        return res.status(500).json({ message: 'Greška prilikom slanja linka za resetiranje lozinke.' });
    }
};

export const getMe = (req, res) => {
    const u = req.user;
    if (!u) return res.status(401).json({ message: 'Unauthorized' });
    res.json({
        _id: u._id,
        name: u.name,
        email: u.email,
        nickname: u.nickname,
        avatarUrl: u.avatarUrl,
        isAdmin: u.isAdmin,
        isUser: u.isUser,
        isVerified: u.isVerified,
        provider: u.provider,
    });
};

export const contactUs = asyncHandler(async (req, res) => {
    const { name, email, message } = req.body || {};
    if (!name || !email || !message) {
        return res.status(400).json({ message: 'Name, email and message are required.' });
    }

    // 🔧 Fallback logika za primatelja
    const TO =
        process.env.SUPPORT_EMAIL ||
        process.env.CONTACT_TO ||
        process.env.SMTP_USER ||
        'bonzobyte@gmail.com';

    if (!TO) {
        console.error('[CONTACT] No recipient configured (SUPPORT_EMAIL/CONTACT_TO/SMTP_USER missing).');
        return res.status(500).json({ message: 'Mail recipient not configured on server.' });
    }

    console.log('[CONTACT] To:', TO, 'From:', email, 'Len:', message?.length);

    try {
        await transporter.sendMail({
            to: TO,
            from: `"BonzoByte Contact" <${process.env.SMTP_USER || TO}>`,
            replyTo: `${name} <${email}>`,
            subject: `Contact form — ${name} <${email}>`,
            text: message,
            html: `
        <p><b>From:</b> ${name} &lt;${email}&gt;</p>
        <pre style="white-space:pre-wrap;font-family:inherit">${message}</pre>
      `,
        });

        console.log('[CONTACT] Mail sent to', TO);
        return res.json({ message: 'Message sent.' });
    } catch (err) {
        console.error('[CONTACT] sendMail error:', err);
        return res.status(500).json({ message: 'Mail sending failed.' });
    }
});


export const updateUserProfile = async (req, res) => {
  try {
    const userId = req.user.id;
    const { nickname } = req.body;

    const updatedData = { nickname };

    if (req.file) {
      updatedData.avatarUrl = `${process.env.BASE_URL}/uploads/${req.file.filename}`;
    }

    const updatedUser = await User.findByIdAndUpdate(userId, updatedData, { new: true });

    if (!updatedUser) {
      return res.status(404).json({ message: 'Korisnik nije pronađen' });
    }

    res.status(200).json({ user: updatedUser });
  } catch (err) {
    console.error(err);
  
    if (err.code === 11000 && err.keyPattern?.nickname) {
      return res.status(409).json({ message: 'Taj nadimak je već zauzet.' });
    }
  
    res.status(500).json({ message: 'Greška pri ažuriranju profila.' });
  }
};