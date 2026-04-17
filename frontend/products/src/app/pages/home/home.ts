import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';

@Component({
  standalone: true,
  templateUrl: './home.html',
  styleUrl: './home.css',
  imports: [CommonModule],
})
export default class HomeComponent implements OnInit {
  ngOnInit(): void {}
}
